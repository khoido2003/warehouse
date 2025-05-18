'use client';

import { useEffect, useRef, useState } from 'react';
import Chart from 'chart.js/auto';
import { TooltipItem } from 'chart.js';

interface OrderItemChartProps {
  data: any[];
}

interface StateGroupedData {
  [key: string]: {
    totalAmount: number;
    unitsSold: number;
    customerCount: number;
  };
}

const OrderItemChart = ({ data }: OrderItemChartProps) => {
  const chartRef = useRef<Chart | null>(null);
  const canvasRef = useRef<HTMLCanvasElement | null>(null);
  const [error, setError] = useState<string | null>(null);
  
  useEffect(() => {
    console.log('OrderItemChart data:', data);
    if (!data || data.length === 0) {
      console.log('OrderItemChart: No data, skipping chart render');
      setError('No data available for chart');
      return;
    }

    try {
      renderStateDistributionChart();
    } catch (err: unknown) {
      console.error('OrderItemChart error:', err);
      setError(err instanceof Error ? err.message : 'Failed to render chart');
    }

    return () => {
      if (chartRef.current) {
        chartRef.current.destroy();
      }
    };
  }, [data]);
  
  const renderStateDistributionChart = () => {
    if (!canvasRef.current) return;
    
    if (chartRef.current) {
      chartRef.current.destroy();
    }

    const ctx = canvasRef.current.getContext('2d');
    if (!ctx) {
      throw new Error('Failed to get canvas context');
    }

    // Process data by state
    const processedData = processStateData(data);
    
    const chartConfig = {
      type: 'pie' as const,
      data: {
        labels: processedData.labels,
        datasets: [
          {
            label: 'Tổng doanh thu',
            data: processedData.totalAmounts,
            backgroundColor: [
              'rgba(54, 162, 235, 0.6)',
              'rgba(255, 99, 132, 0.6)',
              'rgba(255, 206, 86, 0.6)',
              'rgba(75, 192, 192, 0.6)',
              'rgba(153, 102, 255, 0.6)',
              'rgba(255, 159, 64, 0.6)',
              'rgba(199, 199, 199, 0.6)',
              'rgba(83, 102, 255, 0.6)',
              'rgba(40, 159, 64, 0.6)',
              'rgba(210, 199, 199, 0.6)',
            ],
            borderColor: [
              'rgba(54, 162, 235, 1)',
              'rgba(255, 99, 132, 1)',
              'rgba(255, 206, 86, 1)',
              'rgba(75, 192, 192, 1)',
              'rgba(153, 102, 255, 1)',
              'rgba(255, 159, 64, 1)',
              'rgba(199, 199, 199, 1)',
              'rgba(83, 102, 255, 1)',
              'rgba(40, 159, 64, 1)',
              'rgba(210, 199, 199, 1)',
            ],
            borderWidth: 1
          }
        ]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: { 
          legend: { 
            display: true,
            position: 'right' as const
          }, 
          title: { 
            display: true, 
            text: 'Phân bố doanh thu theo khu vực',
            font: {
              size: 16
            }
          },
          tooltip: {
            callbacks: {
              label: function(context: TooltipItem<'pie'>) {
                const label = context.label || '';
                const value = context.parsed as number;
                const dataset = context.dataset;
                const total = dataset.data.reduce((sum: number, data: number) => sum + data, 0);
                const percentage = Math.round((value / total) * 100);
                return `${label}: ${value.toLocaleString()} đ (${percentage}%)`;
              }
            }
          }
        }
      }
    };

    chartRef.current = new Chart(ctx, chartConfig);
  };
  
  // Process data for state distribution chart
  const processStateData = (data: any[]) => {
    // Group data by state
    const stateGroupedData: StateGroupedData = {};
    
    data.forEach(item => {
      const state = item.state?.toString() || 'Unknown';
      
      if (!stateGroupedData[state]) {
        stateGroupedData[state] = {
          totalAmount: 0,
          unitsSold: 0,
          customerCount: 0
        };
      }
      
      stateGroupedData[state].totalAmount += parseFloat(item.totalAmount || '0');
      stateGroupedData[state].unitsSold += parseFloat(item.unitSold || '0');
      
      // Track unique customers per state
      stateGroupedData[state].customerCount += 1;
    });
    
    // Sort by totalAmount (descending)
    const sortedKeys = Object.keys(stateGroupedData).sort((a, b) => 
      stateGroupedData[b].totalAmount - stateGroupedData[a].totalAmount
    );
    
    const labels = sortedKeys;
    const totalAmounts = sortedKeys.map(key => stateGroupedData[key].totalAmount);
    
    return {
      labels,
      totalAmounts
    };
  };

  return (
    <div className="w-full mb-6">
      <div className="bg-white p-6 rounded-lg shadow-md">
        <h2 className="text-xl font-bold mb-4">Biểu đồ phân bố doanh thu theo khu vực</h2>
        {error ? (
          <div className="text-red-600 mb-4">Error: {error}</div>
        ) : (
          <div style={{ height: '500px' }}>
            <canvas ref={canvasRef} />
          </div>
        )}
      </div>
    </div>
  );
};

export default OrderItemChart; 