'use client';

import { useEffect, useRef, useState } from 'react';
import Chart from 'chart.js/auto';

interface SalesChartProps {
  data: any[];
}

const SalesChart = ({ data }: SalesChartProps) => {
  const chartRef = useRef<Chart | null>(null);
  const canvasRef = useRef<HTMLCanvasElement | null>(null);
  const [error, setError] = useState<string | null>(null);

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
        // Requirement 1: Bar chart for product quantities by store/city
        const labels = data.map((item) => `${item.storeId || 'Unknown'} (${item.city || 'Unknown'})`);
        const values = data.map((item) => item.quantity || item.Quantity || 0);

        chartConfig = {
          type: 'bar',
          data: {
            labels,
            datasets: [
              {
                label: 'Product Quantity',
                data: values,
                backgroundColor: 'rgba(54, 162, 235, 0.6)',
                borderColor: 'rgba(54, 162, 235, 1)',
                borderWidth: 1,
              },
            ],
          },
          options: {
            scales: {
              y: { beginAtZero: true, title: { display: true, text: 'Quantity' } },
              x: { title: { display: true, text: 'Store (City)' } },
            },
            plugins: { legend: { display: true }, title: { display: true, text: 'Product Quantities by Store' } },
          },
        };
      }

      if (chartConfig.data.datasets[0].data.every((v: number) => v === 0)) {
        throw new Error('No valid data for chart');
      }

      chartRef.current = new Chart(ctx, chartConfig);
      console.log('SalesChart: Chart initialized with config:', chartConfig);
    } catch (err) {
      console.error('SalesChart error:', err);
      setError(err.message || 'Failed to render chart');
    }

    return () => {
      if (chartRef.current) {
        chartRef.current.destroy();
      }
    };
  }, [data]);

  return (
    <div className="bg-white p-6 rounded-lg shadow-md mb-6">
      {error ? (
        <div className="text-red-600 mb-4">Error: {error}</div>
      ) : (
        <canvas ref={canvasRef} style={{ maxHeight: '400px', width: '100%' }} />
      )}
    </div>
  );
};

export default SalesChart;
