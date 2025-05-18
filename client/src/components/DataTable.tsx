"use client";

import { Dispatch, SetStateAction } from "react";

interface DataTableProps {
  data: any[];
  total: number;
  pageNumber: number;
  setPageNumber: Dispatch<SetStateAction<number>>;
  pageSize: number;
}

const DataTable = ({
  data,
  total,
  pageNumber,
  setPageNumber,
  pageSize,
}: DataTableProps) => {
  if (!data || data.length === 0)
    return <p className="text-gray-500">No data available</p>;

  const totalPages = Math.ceil(total / pageSize);

  return (
    <div className="bg-white p-6 rounded-lg shadow-md">
      <div className="overflow-x-auto">
        <table className="min-w-full border">
          <thead className="bg-gray-100">
            <tr>
              {Object.keys(data[0]).map((key) => (
                <th
                  key={key}
                  className="border p-3 text-left text-sm font-semibold"
                >
                  {key}
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {data.map((row, index) => (
              <tr key={index} className="border-b hover:bg-gray-50">
                {Object.values(row).map((value, i) => (
                  <td key={i} className="border p-3 text-sm">
                    {value}
                  </td>
                ))}
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      <div className="flex justify-between items-center mt-4">
        <button
          onClick={() => setPageNumber((prev) => Math.max(prev - 1, 1))}
          disabled={pageNumber === 1}
          className="bg-blue-600 text-white px-4 py-2 rounded disabled:bg-gray-400 hover:bg-blue-700 transition"
        >
          Previous
        </button>
        <span className="text-sm">
          Page {pageNumber} of {totalPages}
        </span>
        <button
          onClick={() =>
            setPageNumber((prev) => Math.min(prev + 1, totalPages))
          }
          disabled={pageNumber === totalPages}
          className="bg-blue-600 text-white px-4 py-2 rounded disabled:bg-gray-400 hover:bg-blue-700 transition"
        >
          Next
        </button>
      </div>
    </div>
  );
};

export default DataTable;
