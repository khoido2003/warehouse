"use client";

import { Dispatch, SetStateAction } from "react";

interface FiltersProps {
  filters: { city: string; state: string; time: string };
  setFilters: Dispatch<
    SetStateAction<{ city: string; state: string; time: string }>
  >;
  onApply: () => void;
}

const Filters = ({ filters, setFilters, onApply }: FiltersProps) => {
  return (
    <div className="bg-white p-6 rounded-lg shadow-md mb-6">
      <h3 className="text-lg font-semibold mb-4">Filters (Slice & Dice)</h3>
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <input
          type="text"
          placeholder="City"
          value={filters.city}
          onChange={(e) => setFilters({ ...filters, city: e.target.value })}
          className="border p-2 rounded focus:outline-none focus:ring-2 focus:ring-blue-600"
        />
        <input
          type="text"
          placeholder="State"
          value={filters.state}
          onChange={(e) => setFilters({ ...filters, state: e.target.value })}
          className="border p-2 rounded focus:outline-none focus:ring-2 focus:ring-blue-600"
        />
        <input
          type="text"
          placeholder="Time (YYYY-MM-DD)"
          value={filters.time}
          onChange={(e) => setFilters({ ...filters, time: e.target.value })}
          className="border p-2 rounded focus:outline-none focus:ring-2 focus:ring-blue-600"
        />
      </div>
      <button
        onClick={onApply}
        className="mt-4 bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 transition"
      >
        Apply Filters
      </button>
    </div>
  );
};

export default Filters;
