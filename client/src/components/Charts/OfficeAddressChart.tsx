'use client';

import { useEffect, useRef, useState } from 'react';
import Chart from 'chart.js/auto';
import { ChartConfiguration, TooltipItem } from 'chart.js';

interface OfficeAddressChartProps {
  data: any[];
}

const OfficeAddressChart = ({ data }: OfficeAddressChartProps) => {
  const chartRef = useRef<Chart | null>(null);
  const canvasRef = useRef<HTMLCanvasElement | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [chartType, setChartType] = useState<'pie' | 'bar'>('pie');

  useEffect(() => {
    console.log('OfficeAddressChart data:', data);
    if (!data || data.length === 0 || !canvasRef.current) {
      console.log('OfficeAddressChart: No data or canvas, skipping chart render');
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

      // Dữ liệu thống kê theo bang
      const stateData = processDataByState(data);
      
      // Tạo biểu đồ theo bang
      const chartConfig: ChartConfiguration<'bar' | 'pie', number[], string> = {
        type: chartType,
        data: {
          labels: stateData.states,
          datasets: [{
            label: 'Số lượng văn phòng',
            data: chartType === 'pie' ? stateData.percentages : stateData.counts,
            backgroundColor: stateData.states.map((_, index) => {
              const hue = (index * 25) % 360;
              return `hsla(${hue}, 70%, 60%, 0.7)`;
            }),
            borderColor: stateData.states.map((_, index) => {
              const hue = (index * 25) % 360;
              return `hsla(${hue}, 70%, 60%, 1)`;
            }),
            borderWidth: 1,
          }]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          ...(chartType === 'bar' && {
            scales: {
              y: { 
                beginAtZero: true, 
                title: { display: true, text: 'Số lượng văn phòng' } 
              },
              x: { 
                title: { display: true, text: 'Bang' },
                ticks: {
                  autoSkip: false,
                  maxRotation: 45,
                  minRotation: 45
                }
              },
            }
          }),
          plugins: { 
            legend: { 
              display: true,
              position: chartType === 'pie' ? 'right' : 'top',
              labels: {
                font: {
                  size: 12
                }
              }
            }, 
            title: { 
              display: true, 
              text: 'Phân bố văn phòng theo bang',
              font: {
                size: 16
              }
            },
            tooltip: {
              callbacks: {
                label: function(context: TooltipItem<'bar' | 'pie'>) {
                  const value = context.raw as number;
                  if (chartType === 'pie') {
                    return `${context.label}: ${value.toFixed(1)}%`;
                  } else {
                    return `${context.label}: ${value} văn phòng`;
                  }
                }
              }
            }
          },
        },
      };

      chartRef.current = new Chart(ctx, chartConfig);
      
    } catch (err: unknown) {
      console.error('OfficeAddressChart error:', err);
      setError(err instanceof Error ? err.message : 'Failed to render chart');
    }

    return () => {
      if (chartRef.current) {
        chartRef.current.destroy();
      }
    };
  }, [data, chartType]);

  // Hàm xử lý dữ liệu thống kê theo bang
  const processDataByState = (data: any[]) => {
    // Nhóm dữ liệu theo bang
    const stateGroups: { [key: string]: number } = {};
    
    data.forEach(item => {
      const state = item.state || 'Unknown';
      if (stateGroups[state]) {
        stateGroups[state]++;
      } else {
        stateGroups[state] = 1;
      }
    });
    
    // Sắp xếp theo số lượng giảm dần
    const sortedStates = Object.entries(stateGroups)
      .sort((a, b) => b[1] - a[1])
      .map(entry => ({ state: entry[0], count: entry[1] }));

    // Tính tổng và phần trăm
    const total = sortedStates.reduce((sum, item) => sum + item.count, 0);
    const percentages = sortedStates.map(item => (item.count / total) * 100);
    
    return {
      states: sortedStates.map(item => item.state),
      counts: sortedStates.map(item => item.count),
      percentages: percentages,
      total: total
    };
  };

  return (
    <div className="bg-white p-6 rounded-lg shadow-md mb-6">
      {error ? (
        <div className="text-red-600 mb-4">Error: {error}</div>
      ) : (
        <>
          <div className="flex justify-between mb-4">
            <div className="text-gray-700">
              Tổng số: {data.length} văn phòng trên {Array.from(new Set(data.map(item => item.state))).length} bang
            </div>
            <div className="inline-flex rounded-md shadow-sm" role="group">
              <button
                type="button"
                onClick={() => setChartType('pie')}
                className={`px-4 py-2 text-sm font-medium rounded-l-lg ${
                  chartType === 'pie' 
                    ? 'bg-blue-600 text-white' 
                    : 'bg-white text-gray-700 hover:bg-gray-100'
                }`}
              >
                Pie
              </button>
              <button
                type="button"
                onClick={() => setChartType('bar')}
                className={`px-4 py-2 text-sm font-medium rounded-r-lg ${
                  chartType === 'bar' 
                    ? 'bg-blue-600 text-white' 
                    : 'bg-white text-gray-700 hover:bg-gray-100'
                }`}
              >
                Bar
              </button>
            </div>
          </div>
          
          <div style={{ height: '500px' }}>
            <canvas ref={canvasRef} />
          </div>
        </>
      )}
    </div>
  );
};

export default OfficeAddressChart; 