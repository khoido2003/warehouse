'use client';

import { useEffect, useRef, useState } from 'react';
import Chart from 'chart.js/auto';
import { TooltipItem } from 'chart.js';

interface SalesChartProps {
  data: any[];
}

const SalesChart = ({ data }: SalesChartProps) => {
  const chartRef = useRef<Chart | null>(null);
  const canvasRef = useRef<HTMLCanvasElement | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [chartType, setChartType] = useState<'bar' | 'pie'>('bar');

  useEffect(() => {
    console.log('SalesChart data:', data);
    if (!data || data.length === 0 || !canvasRef.current) {
      console.log('SalesChart: No data or canvas, skipping chart render');
      setError('No data available for chart');
      return;
    }

    try {
      if (chartRef.current) {
        chartRef.current.destroy();
      }

      const ctx = canvasRef.current.getContext('2d');
      if (!ctx) {
        throw new Error('Failed to get canvas context');
      }

      let chartConfig: any;
      if (data[0]?.orderDate) {
        // Requirement 2: Line chart for orders over time
        const dates = [...new Set(data.map((item) => item.orderDate.split('T')[0]))].sort();
        const totalAmounts = dates.map((date) =>
          data
            .filter((item) => item.orderDate.split('T')[0] === date)
            .reduce((sum, item) => sum + (item.totalAmount || item.total_amount || 0), 0)
        );

        chartConfig = {
          type: 'line',
          data: {
            labels: dates,
            datasets: [
              {
                label: 'Total Order Amount',
                data: totalAmounts,
                borderColor: 'rgba(54, 162, 235, 1)',
                backgroundColor: 'rgba(54, 162, 235, 0.2)',
                fill: false,
                tension: 0.4,
              },
            ],
          },
          options: {
            scales: {
              y: { beginAtZero: true, title: { display: true, text: 'Amount ($)' } },
              x: { title: { display: true, text: 'Date' } },
            },
            plugins: { legend: { display: true }, title: { display: true, text: 'Order Trends Over Time' } },
          },
        };
      } else {
        // Requirement 1: Aggregate data by city
        const cityMap = new Map<string, number>();
        
        // Group by city and sum quantities
        data.forEach(item => {
          const city = item.city || 'Unknown';
          const quantity = item.quantity || 0;
          
          if (cityMap.has(city)) {
            cityMap.set(city, cityMap.get(city)! + quantity);
          } else {
            cityMap.set(city, quantity);
          }
        });
        
        // Sort cities by quantity (descending)
        const sortedCities = Array.from(cityMap.entries())
          .sort((a, b) => b[1] - a[1]);
        
        let labels: string[] = [];
        let values: number[] = [];
        let backgroundColors: string[] = [];
        
        // For both bar and pie charts, display all cities without grouping
        labels = sortedCities.map(item => item[0]);
        values = sortedCities.map(item => item[1]);
        
        // Generate colors
        backgroundColors = labels.map((_, index) => {
          const hue = (index * 30) % 360;
          return `hsla(${hue}, 70%, 60%, 0.7)`;
        });

        // Use the selected chart type
        chartConfig = {
          type: chartType,
          data: {
            labels,
            datasets: [
              {
                label: 'Total Product Quantity',
                data: values,
                backgroundColor: backgroundColors,
                borderColor: backgroundColors.map(color => color.replace('0.7', '1')),
                borderWidth: 1,
              },
            ],
          },
          options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: { 
              legend: { 
                display: true,
                position: chartType === 'pie' ? 'right' as const : 'top' as const,
                // Customize legend for pie chart to handle many items
                ...(chartType === 'pie' && {
                  maxHeight: 300,
                  labels: {
                    boxWidth: 10,
                    font: {
                      size: 10
                    }
                  }
                })
              }, 
              title: { 
                display: true, 
                text: 'Total Product Quantities by City',
                font: {
                  size: 16
                }
              },
              tooltip: {
                callbacks: {
                  label: function(context: TooltipItem<typeof chartType>) {
                    const value = context.raw as number;
                    let sum = 0;
                    const dataArr = context.chart.data.datasets[0].data as number[];
                    dataArr.forEach(val => {
                      sum += val;
                    });
                    const percentage = ((value * 100) / sum).toFixed(2);
                    return `${context.label}: ${value} (${percentage}%)`;
                  }
                }
              }
            },
          },
        };
        
        // Add axis labels for bar chart
        if (chartType === 'bar') {
          chartConfig.options.scales = {
            y: { 
              beginAtZero: true, 
              title: { display: true, text: 'Quantity' } 
            },
            x: { 
              title: { display: true, text: 'City' },
              ticks: {
                autoSkip: false,
                maxRotation: 90,
                minRotation: 45
              }
            },
          };
          
          // Adjust chart height based on number of cities
          const minHeight = 400;
          const heightPerCity = 20; // pixels per city
          const dynamicHeight = Math.max(minHeight, sortedCities.length * heightPerCity);
          chartConfig.options.maintainAspectRatio = false;
          
          // Update chart container height
          const chartContainer = canvasRef.current?.parentElement;
          if (chartContainer) {
            chartContainer.style.height = `${dynamicHeight}px`;
            chartContainer.style.overflowY = sortedCities.length > 20 ? 'auto' : 'visible';
          }
        }
        // Adjust for pie chart with many slices
        else if (chartType === 'pie') {
          // Increase chart size when there are many items
          const chartContainer = canvasRef.current?.parentElement;
          if (chartContainer) {
            chartContainer.style.height = '500px';
            // If there are many cities, we need scrollable legend
            if (sortedCities.length > 15) {
              chartConfig.options.plugins.legend.position = 'right';
              chartConfig.options.plugins.legend.overflow = 'auto';
              chartConfig.options.plugins.legend.maxHeight = 450;
            }
          }
        }
      }

      if (!chartConfig.data.datasets[0].data || chartConfig.data.datasets[0].data.every((v: number) => v === 0)) {
        throw new Error('No valid data for chart');
      }

      chartRef.current = new Chart(ctx, chartConfig);
      console.log('SalesChart: Chart initialized with config:', chartConfig);
    } catch (err: unknown) {
      console.error('SalesChart error:', err);
      setError(err instanceof Error ? err.message : 'Failed to render chart');
    }

    return () => {
      if (chartRef.current) {
        chartRef.current.destroy();
      }
    };
  }, [data, chartType]);

  return (
    <div className="bg-white p-6 rounded-lg shadow-md mb-6">
      {error ? (
        <div className="text-red-600 mb-4">Error: {error}</div>
      ) : (
        <>
          {!data[0]?.orderDate && (
            <div className="flex justify-between mb-4">
              <div className="text-sm text-gray-500">
                {data.length > 0 ? 
                  `Hiển thị tất cả ${Array.from(new Set(data.map(item => item.city))).filter(city => city).length} thành phố` : ''}
              </div>
              <div className="inline-flex rounded-md shadow-sm" role="group">
                <button
                  type="button"
                  onClick={() => setChartType('bar')}
                  className={`px-4 py-2 text-sm font-medium rounded-l-lg ${
                    chartType === 'bar' 
                      ? 'bg-blue-600 text-white' 
                      : 'bg-white text-gray-700 hover:bg-gray-100'
                  }`}
                >
                  Bar
                </button>
                <button
                  type="button"
                  onClick={() => setChartType('pie')}
                  className={`px-4 py-2 text-sm font-medium rounded-r-lg ${
                    chartType === 'pie' 
                      ? 'bg-blue-600 text-white' 
                      : 'bg-white text-gray-700 hover:bg-gray-100'
                  }`}
                >
                  Pie
                </button>
              </div>
            </div>
          )}
          <div className={`chart-container ${chartType === 'bar' ? 'overflow-x-auto' : ''}`} style={{
            height: chartType === 'bar' ? 'auto' : '500px',
            maxHeight: chartType === 'bar' ? '70vh' : '500px'
          }}>
            <canvas ref={canvasRef} />
          </div>
        </>
      )}
    </div>
  );
};

export default SalesChart;
