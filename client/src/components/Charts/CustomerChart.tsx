'use client';

import { useEffect, useRef, useState } from 'react';
import Chart from 'chart.js/auto';

interface CustomerChartProps {
  data: any[];
}

const CustomerChart = ({ data }: CustomerChartProps) => {
  const chartRef = useRef<Chart | null>(null);
  const canvasRef = useRef<HTMLCanvasElement | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    console.log('CustomerChart data:', data);
    if (!data || data.length === 0 || !canvasRef.current) {
      console.log('CustomerChart: No data or canvas, skipping chart render');
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

      // Count customer types
      const travelOnly = data.filter((item) => (item.isTravel || item.travel) && !(item.isPost || item.post)).length;
      const postOnly = data.filter((item) => !(item.isTravel || item.travel) && (item.isPost || item.post)).length;
      const both = data.filter((item) => (item.isTravel || item.travel) && (item.isPost || item.post)).length;

      if (travelOnly === 0 && postOnly === 0 && both === 0) {
        throw new Error('No valid customer type data');
      }

      chartRef.current = new Chart(ctx, {
        type: 'pie',
        data: {
          labels: ['Travel Only', 'Post Only', 'Both'],
          datasets: [
            {
              label: 'Customer Types',
              data: [travelOnly, postOnly, both],
              backgroundColor: ['rgba(255, 99, 132, 0.6)', 'rgba(54, 162, 235, 0.6)', 'rgba(75, 192, 192, 0.6)'],
              borderColor: ['rgba(255, 99, 132, 1)', 'rgba(54, 162, 235, 1)', 'rgba(75, 192, 192, 1)'],
              borderWidth: 1,
            },
          ],
        },
        options: {
          plugins: { legend: { display: true }, title: { display: true, text: 'Customer Type Distribution' } },
        },
      });

      console.log('CustomerChart: Chart initialized with data:', { travelOnly, postOnly, both });
    } catch (err) {
      console.error('CustomerChart error:', err);
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

export default CustomerChart;
