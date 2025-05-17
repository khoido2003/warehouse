'use client';

import { useEffect, useRef } from 'react';
import Chart from 'chart.js/auto';

interface InventoryChartProps {
  data: any[];
}

const InventoryChart = ({ data }: InventoryChartProps) => {
  const chartRef = useRef<Chart | null>(null);
  const canvasRef = useRef<HTMLCanvasElement | null>(null);

  useEffect(() => {
    if (data && data.length > 0 && canvasRef.current) {
      if (chartRef.current) {
        chartRef.current.destroy();
      }

      const ctx = canvasRef.current.getContext('2d');
      if (ctx) {
        chartRef.current = new Chart(ctx, {
          type: 'bar',
          data: {
            labels: data.map((item) => item.City_name || 'Unknown'),
            datasets: [
              {
                label: 'Inventory Quantity',
                data: data.map((item) => item.Quantity || 0),
                backgroundColor: 'rgba(75, 192, 192, 0.6)',
                borderColor: 'rgba(75, 192, 192, 1)',
                borderWidth: 1,
              },
            ],
          },
          options: {
            scales: {
              y: {
                beginAtZero: true,
                title: { display: true, text: 'Quantity' },
              },
              x: {
                title: { display: true, text: 'City' },
              },
            },
            plugins: {
              legend: { display: true },
              title: { display: true, text: 'Inventory Levels by City' },
            },
          },
        });
      }
    }

    return () => {
      if (chartRef.current) {
        chartRef.current.destroy();
      }
    };
  }, [data]);

  return (
    <div className="bg-white p-6 rounded-lg shadow-md mb-6">
      <canvas ref={canvasRef}></canvas>
    </div>
  );
};

export default InventoryChart;
