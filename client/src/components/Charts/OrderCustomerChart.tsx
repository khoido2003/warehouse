'use client';

import { useEffect, useRef, useState } from 'react';
import Chart from 'chart.js/auto';
import { TooltipItem } from 'chart.js';

interface OrderCustomerChartProps {
  data: any[];
}

const OrderCustomerChart = ({ data }: OrderCustomerChartProps) => {
  const chartRef = useRef<Chart | null>(null);
  const canvasRef = useRef<HTMLCanvasElement | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    console.log('OrderCustomerChart data:', data);
    if (!data || data.length === 0 || !canvasRef.current) {
      console.log('OrderCustomerChart: No data or canvas, skipping chart render');
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

      // Tính toán dữ liệu tổng doanh thu theo năm
      const totalByYear = processDataByYear(data);
      
      // Tạo biểu đồ cột
      const chartConfig = {
        type: 'bar' as const,
        data: {
          labels: totalByYear.years,
          datasets: [{
            label: 'Tổng doanh thu',
            data: totalByYear.amounts,
            backgroundColor: 'rgba(54, 162, 235, 0.6)',
            borderColor: 'rgba(54, 162, 235, 1)',
            borderWidth: 1,
          }]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          scales: {
            y: { 
              beginAtZero: true, 
              title: { display: true, text: 'Tổng doanh thu' } 
            },
            x: { 
              title: { display: true, text: 'Năm' } 
            },
          },
          plugins: { 
            legend: { 
              display: true,
              position: 'top' as const,
            }, 
            title: { 
              display: true, 
              text: 'Tổng doanh thu theo năm',
              font: {
                size: 16
              }
            },
            tooltip: {
              callbacks: {
                label: function(context: TooltipItem<'bar'>) {
                  const value = context.raw as number;
                  return `Doanh thu: ${value.toLocaleString()} đ`;
                }
              }
            }
          },
        },
      };

      chartRef.current = new Chart(ctx, chartConfig);
      console.log('OrderCustomerChart: Chart initialized with config:', chartConfig);
    } catch (err: unknown) {
      console.error('OrderCustomerChart error:', err);
      setError(err instanceof Error ? err.message : 'Failed to render chart');
    }

    return () => {
      if (chartRef.current) {
        chartRef.current.destroy();
      }
    };
  }, [data]);

  // Hàm xử lý dữ liệu tính tổng doanh thu theo năm
  const processDataByYear = (data: any[]) => {
    // Lấy tất cả các năm độc nhất và sắp xếp
    const years = Array.from(new Set(data.map(item => item.year)))
      .sort((a, b) => a - b)
      .map(year => year.toString());
    
    // Tính tổng doanh thu cho mỗi năm
    const amounts = years.map(year => {
      const yearData = data.filter(item => item.year.toString() === year);
      return yearData.reduce((total, item) => total + (parseFloat(item.totalAmount) || 0), 0);
    });

    return { years, amounts };
  };

  return (
    <div className="bg-white p-6 rounded-lg shadow-md mb-6">
      {error ? (
        <div className="text-red-600 mb-4">Error: {error}</div>
      ) : (
        <div style={{ height: '500px' }}>
          <canvas ref={canvasRef} />
        </div>
      )}
    </div>
  );
};

export default OrderCustomerChart; 