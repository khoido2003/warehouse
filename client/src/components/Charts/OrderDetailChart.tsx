'use client';

import { useEffect, useRef, useState } from 'react';
import Chart from 'chart.js/auto';
import { 
  CategoryScale, 
  LinearScale, 
  BarController, 
  BarElement, 
  Tooltip, 
  Legend, 
  Title,
  ChartConfiguration,
  TooltipCallbacks,
  ChartTypeRegistry
} from 'chart.js';

Chart.register(CategoryScale, LinearScale, BarController, BarElement, Tooltip, Legend, Title);

interface OrderDetailChartProps {
  data: any[];
}

type ChartViewType = 'product' | 'time';

const OrderDetailChart = ({ data }: OrderDetailChartProps) => {
  const chartRef = useRef<Chart | null>(null);
  const canvasRef = useRef<HTMLCanvasElement | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [chartView, setChartView] = useState<ChartViewType>('product');

  useEffect(() => {
    if (!data || data.length === 0 || !canvasRef.current) {
      setError('No data available for chart');
      return;
    }

    // Check if time data is available and set default view accordingly
    const hasTimeData = data.some(item => item.month || item.quarter || item.year);
    if (!hasTimeData && chartView === 'time') {
      setChartView('product'); // Default to product view if time data is not available
      return; // This will trigger a re-render with the new view
    }

    try {
      if (chartRef.current) {
        chartRef.current.destroy();
      }

      const ctx = canvasRef.current.getContext('2d');
      if (!ctx) {
        throw new Error('Failed to get canvas context');
      }

      // Prepare data based on view type
      let chartData;
      let chartType: 'bar' | 'line' = 'bar';
      
      if (chartView === 'product') {
        // Group by product
        chartData = prepareProductData(data);
      } else {
        // Group by time
        chartData = prepareTimeData(data);
        chartType = 'line';
      }

      // Create chart configuration based on view type
      const chartConfig = createChartConfig(chartData, chartType, chartView);

      // Create chart
      chartRef.current = new Chart(ctx, chartConfig as any);

      setError(null);
    } catch (err) {
      console.error('OrderDetailChart error:', err);
      setError(`Failed to create chart: ${err instanceof Error ? err.message : 'Unknown error'}`);
    }
  }, [data, chartView]);

  // Create chart configuration based on data and view type
  const createChartConfig = (chartData: any, chartType: 'bar' | 'line', viewType: string) => {
    if (viewType === 'time') {
      return {
        type: chartType,
        data: {
          labels: chartData.labels,
          datasets: [
            {
              label: 'Total Amount ($)',
              data: chartData.amounts,
              backgroundColor: 'rgba(54, 162, 235, 0.7)',
              borderColor: 'rgba(54, 162, 235, 1)',
              borderWidth: 2,
              fill: false,
              tension: 0.4,
              yAxisID: 'y'
            },
            {
              label: 'Units Sold',
              data: chartData.units,
              backgroundColor: 'rgba(255, 99, 132, 0.7)',
              borderColor: 'rgba(255, 99, 132, 1)',
              borderWidth: 2,
              fill: false,
              tension: 0.4,
              yAxisID: 'y1'
            }
          ]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          scales: {
            x: {
              title: {
                display: true,
                text: 'Time Period',
                font: {
                  weight: 'bold'
                }
              }
            },
            y: {
              type: 'linear' as const,
              display: true,
              position: 'left' as const,
              title: {
                display: true,
                text: 'Total Amount ($)',
                font: {
                  weight: 'bold'
                }
              },
              beginAtZero: true
            },
            y1: {
              type: 'linear' as const,
              display: true,
              position: 'right' as const,
              title: {
                display: true,
                text: 'Units Sold',
                font: {
                  weight: 'bold'
                }
              },
              beginAtZero: true,
              grid: {
                drawOnChartArea: false
              }
            }
          },
          plugins: {
            legend: {
              display: true,
              position: 'top' as const
            },
            title: {
              display: true,
              text: 'Order Trends Over Time',
              font: {
                size: 16,
                weight: 'bold'
              }
            },
            tooltip: {
              callbacks: {
                label: function(context: any) {
                  const label = context.dataset.label || '';
                  const value = context.parsed.y;
                  return `${label}: ${value.toLocaleString()}`;
                }
              } as any
            }
          }
        }
      };
    } else {
      // Standard bar chart for product or store view
      return {
        type: chartType,
        data: {
          labels: chartData.labels,
          datasets: [
            {
              label: 'Total Amount ($)',
              data: chartData.amounts,
              backgroundColor: 'rgba(54, 162, 235, 0.7)',
              borderColor: 'rgba(54, 162, 235, 1)',
              borderWidth: 1,
              yAxisID: 'y'
            },
            {
              label: 'Units Sold',
              data: chartData.units,
              backgroundColor: 'rgba(255, 99, 132, 0.7)',
              borderColor: 'rgba(255, 99, 132, 1)',
              borderWidth: 1,
              yAxisID: 'y1'
            }
          ]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          scales: {
            x: {
              title: {
                display: true,
                text: 'Products',
                font: {
                  weight: 'bold'
                }
              },
              ticks: {
                maxRotation: 45,
                minRotation: 45
              }
            },
            y: {
              type: 'linear' as const,
              display: true,
              position: 'left' as const,
              title: {
                display: true,
                text: 'Total Amount ($)',
                font: {
                  weight: 'bold'
                }
              },
              beginAtZero: true
            },
            y1: {
              type: 'linear' as const,
              display: true,
              position: 'right' as const,
              title: {
                display: true,
                text: 'Units Sold',
                font: {
                  weight: 'bold'
                }
              },
              beginAtZero: true,
              grid: {
                drawOnChartArea: false
              }
            }
          },
          plugins: {
            legend: {
              display: true,
              position: 'top' as const
            },
            title: {
              display: true,
              text: viewType === 'product' ? 'Order Details by Product' : 'Order Trends Over Time',
              font: {
                size: 16,
                weight: 'bold'
              }
            },
            tooltip: {
              callbacks: {
                label: function(context: any) {
                  const label = context.dataset.label || '';
                  const value = context.parsed.y;
                  return `${label}: ${value.toLocaleString()}`;
                }
              } as any
            }
          }
        }
      };
    }
  };

  // Helper function to prepare product-based data
  const prepareProductData = (data: any[]) => {
    // Group by product and calculate totals
    const productMap = new Map<string, { amount: number, units: number, fullDescription: string }>();
    
    data.forEach((item: any) => {
      const productId = item.productId || 'Unknown';
      const description = item.description || productId;
      const key = productId; // Use productId as the key for grouping
      const amount = parseFloat(item.totalAmount) || 0;
      const units = parseFloat(item.unitSold) || 0;
      
      if (productMap.has(key)) {
        const existing = productMap.get(key)!;
        existing.amount += amount;
        existing.units += units;
      } else {
        productMap.set(key, { amount, units, fullDescription: description });
      }
    });
    
    // Sort by total amount
    const sortedEntries = Array.from(productMap.entries())
      .sort((a, b) => b[1].amount - a[1].amount)
      .slice(0, 10); // Limit to top 10 for clarity
    
    // Create shortened labels
    const labels = sortedEntries.map(entry => {
      const id = entry[0];
      const fullDesc = entry[1].fullDescription;
      
      // Truncate description if longer than 20 characters
      if (fullDesc.length > 20) {
        return `${fullDesc.substring(0, 17)}...`;
      }
      return fullDesc;
    });
    
    return {
      labels: labels,
      amounts: sortedEntries.map(entry => entry[1].amount),
      units: sortedEntries.map(entry => entry[1].units)
    };
  };

  // Helper function to prepare time-based data
  const prepareTimeData = (data: any[]) => {
    // First check if we have month or quarter data
    const hasMonth = data.some(item => item.month);
    const hasQuarter = data.some(item => item.quarter);
    const hasYear = data.some(item => item.year);
    
    // Choose time granularity based on available data
    let timeKey = 'month';
    
    if (!hasMonth && hasQuarter) {
      timeKey = 'quarter';
    } else if (!hasMonth && !hasQuarter && hasYear) {
      timeKey = 'year';
    }
    
    // Group by the selected time period
    const timeMap = new Map<string, { amount: number, units: number, sortKey: string }>();
    
    data.forEach((item: any) => {
      // Create a formatted time label
      let timeLabel = 'Unknown';
      let sortKey = '';
      
      if (timeKey === 'month' && item.month) {
        // Format: "Jan 2023" 
        timeLabel = `${item.month} ${item.year || ''}`;
        // Sort key to ensure chronological order
        const monthNum = getMonthNumber(item.month);
        sortKey = `${item.year || '0000'}-${monthNum.toString().padStart(2, '0')}`;
      } else if (timeKey === 'quarter' && item.quarter) {
        // Format: "Q1 2023"
        timeLabel = `${item.quarter} ${item.year || ''}`;
        // Extract quarter number for sorting
        const quarterNum = parseInt(item.quarter.replace(/\D/g, '')) || 0;
        sortKey = `${item.year || '0000'}-${quarterNum}`;
      } else if (timeKey === 'year' && item.year) {
        timeLabel = item.year;
        sortKey = item.year;
      }
      
      const amount = parseFloat(item.totalAmount) || 0;
      const units = parseFloat(item.unitSold) || 0;
      
      if (timeMap.has(timeLabel)) {
        const existing = timeMap.get(timeLabel)!;
        existing.amount += amount;
        existing.units += units;
      } else {
        timeMap.set(timeLabel, { amount, units, sortKey });
      }
    });
    
    // Sort by chronological order
    const sortedEntries = Array.from(timeMap.entries())
      .sort((a, b) => a[1].sortKey.localeCompare(b[1].sortKey));
    
    return {
      labels: sortedEntries.map(entry => entry[0]),
      amounts: sortedEntries.map(entry => entry[1].amount),
      units: sortedEntries.map(entry => entry[1].units)
    };
  };

  // Helper to convert month name to number for sorting
  const getMonthNumber = (monthName: string): number => {
    const months: {[key: string]: number} = {
      'Jan': 1, 'January': 1,
      'Feb': 2, 'February': 2,
      'Mar': 3, 'March': 3,
      'Apr': 4, 'April': 4,
      'May': 5,
      'Jun': 6, 'June': 6,
      'Jul': 7, 'July': 7,
      'Aug': 8, 'August': 8,
      'Sep': 9, 'September': 9,
      'Oct': 10, 'October': 10,
      'Nov': 11, 'November': 11,
      'Dec': 12, 'December': 12
    };
    
    // Try to match month name or extract numeric part
    const key = Object.keys(months).find(m => 
      monthName.includes(m) || monthName.toLowerCase().includes(m.toLowerCase())
    );
    
    if (key) {
      return months[key];
    }
    
    // If no match, try to extract a number from the string
    const match = monthName.match(/\d+/);
    return match ? parseInt(match[0]) : 0;
  };

  return (
    <div className="bg-white p-6 rounded-lg shadow-md">
      <div className="mb-4 flex justify-between items-center">
        <h3 className="text-lg font-semibold">Order Details Overview</h3>
        <div className="flex space-x-2">
          <button
            className={`px-3 py-1 rounded ${
              chartView === 'product' ? 'bg-blue-600 text-white' : 'bg-gray-200 text-gray-700'
            }`}
            onClick={() => setChartView('product')}
          >
            By Product
          </button>
          {data.some(item => item.month || item.quarter || item.year) && (
            <button
              className={`px-3 py-1 rounded ${
                chartView === 'time' ? 'bg-blue-600 text-white' : 'bg-gray-200 text-gray-700'
              }`}
              onClick={() => setChartView('time')}
            >
              By Time
            </button>
          )}
        </div>
      </div>
      
      {error ? (
        <div className="text-red-500 p-4">{error}</div>
      ) : (
        <div className="h-[500px] relative">
          <canvas ref={canvasRef} />
        </div>
      )}
      
      {data.length > 0 && (
        <div className="mt-4 text-sm text-gray-600">
          <p>
            {chartView === 'product' 
              ? '* Showing top 10 products by sales amount' 
              : '* Showing trends over time'}
          </p>
        </div>
      )}
    </div>
  );
};

export default OrderDetailChart; 