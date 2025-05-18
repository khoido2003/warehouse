"use client";

import { useState, useEffect } from "react";
import axios from "axios";
import Filters from "./Filters";
import DataTable from "./DataTable";
import SalesChart from "./Charts/SalesChart";
import InventoryChart from "./Charts/InventoryChart";
import CustomerChart from "./Charts/CustomerChart";
import Navigation from "./Navigation";

interface DashboardProps {
  requirement: number;
}

const Dashboard = ({ requirement }: DashboardProps) => {
  const [data, setData] = useState<any[]>([]);
  const [total, setTotal] = useState<number>(0);
  const [pageNumber, setPageNumber] = useState<number>(1);
  const [pageSize] = useState<number>(20);
  const [filters, setFilters] = useState<{
    city: string;
    state: string;
    time: string;
  }>({
    city: "",
    state: "",
    time: "",
  });
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const apiEndpoints: { [key: number]: string } = {
    1: "cube1/requirement1",
    2: "cube2/requirement2",
    3: "cube3/requirement3",
    4: "cube1/requirement4",
    5: "cube4/requirement5",
    6: "cube5/requirement6",
    7: "cube1/requirement7",
    8: "cube6/requirement8",
    9: "cube7/requirement9",
  };

  const fetchData = async () => {
    setLoading(true);
    setError(null);
    try {
      const endpoint = apiEndpoints[requirement];
      if (!endpoint) {
        throw new Error(`Invalid requirement: ${requirement}`);
      }
      const params = new URLSearchParams({
        pageNumber: pageNumber.toString(),
        pageSize: pageSize.toString(),
        city: filters.city,
        state: filters.state,
        ...(filters.time && requirement === 2 && { time: filters.time }),
      });

      const response = await axios.get(`/api/${endpoint}?${params.toString()}`);
      console.log(
        `API response for requirement ${requirement}:`,
        response.data,
      );
      if (!response.data.data || !Array.isArray(response.data.data)) {
        throw new Error("Invalid data format: Expected data array");
      }
      setData(response.data.data);
      setTotal(response.data.total || 0);
    } catch (error: any) {
      console.error("Error fetching data:", error);
      setError(error.message || "Failed to fetch data");
      setData([]);
      setTotal(0);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, [requirement, pageNumber, filters]);

  const handleApplyFilters = () => {
    setPageNumber(1);
    fetchData();
  };

  const renderChart = () => {
    if (!data || data.length === 0) {
      console.log("renderChart: No data, skipping chart");
      return null;
    }
    console.log(
      "renderChart: Rendering chart for requirement",
      requirement,
      "with data:",
      data,
    );
    switch (requirement) {
      case 1:
        return <SalesChart data={data} />; // Bar chart: Product quantities by store/city
      case 2:
        return <SalesChart data={data} />; // Line chart: Order totals over time
      case 7:
        return <InventoryChart data={data} />; // Bar chart: Inventory by store/city
      case 9:
        return <CustomerChart data={data} />; // Pie chart: Customer types
      case 3:
      case 4:
      case 5:
      case 6:
      case 8:
        return null; // No chart for these, rely on DataTable
      default:
        return null;
    }
  };

  const renderInsights = () => {
    if (data.length === 0) return null;
    switch (requirement) {
      case 1:
        const highValueProducts = data.filter(
          (item) => parseFloat(item.price || 0) * (item.quantity || 0) > 10000,
        );
        return highValueProducts.length > 0 ? (
          <div className="bg-green-100 p-4 rounded-lg mb-6">
            <h4 className="text-lg font-semibold text-green-800">
              High-Value Inventory
            </h4>
            <p className="text-green-700">
              {highValueProducts.length} products with estimated value (price *
              quantity) above $10,000. Consider prioritizing these.
            </p>
          </div>
        ) : null;
      case 2:
        const highValueOrders = data.filter(
          (item) => (item.totalAmount || item.total_amount || 0) > 1000,
        );
        return highValueOrders.length > 0 ? (
          <div className="bg-green-100 p-4 rounded-lg mb-6">
            <h4 className="text-lg font-semibold text-green-800">
              High-Value Orders
            </h4>
            <p className="text-green-700">
              {highValueOrders.length} orders with total amount above $1,000.
              Great performance!
            </p>
          </div>
        ) : null;
      case 7:
        const lowInventory = data.filter(
          (item) => (item.quantity || item.Quantity || 0) < 50,
        );
        return lowInventory.length > 0 ? (
          <div className="bg-yellow-100 p-4 rounded-lg mb-6">
            <h4 className="text-lg font-semibold text-yellow-800">
              Low Inventory Alert
            </h4>
            <p className="text-yellow-700">
              {lowInventory.length} items with inventory below 50 units in{" "}
              {filters.city || "selected city"}. Consider restocking.
            </p>
          </div>
        ) : (
          <div className="bg-green-100 p-4 rounded-lg mb-6">
            <h4 className="text-lg font-semibold text-green-800">
              Inventory Status
            </h4>
            <p className="text-green-700">
              All items have sufficient inventory in{" "}
              {filters.city || "selected city"}.
            </p>
          </div>
        );
      case 9:
        const travelCustomers = data.filter(
          (item) =>
            (item.isTravel || item.travel) && !(item.isPost || item.post),
        ).length;
        const postCustomers = data.filter(
          (item) =>
            !(item.isTravel || item.travel) && (item.isPost || item.post),
        ).length;
        return (
          <div className="bg-blue-100 p-4 rounded-lg mb-6">
            <h4 className="text-lg font-semibold text-blue-800">
              Customer Segmentation
            </h4>
            <p className="text-blue-700">
              {travelCustomers} travel-only customers, {postCustomers} post-only
              customers. Target marketing accordingly.
            </p>
          </div>
        );
      default:
        return null;
    }
  };

  return (
    <div className="min-h-screen bg-gray-100">
      <Navigation />
      <div className="container mx-auto p-6">
        <h2 className="text-2xl font-bold mb-6">Requirement {requirement}</h2>
        <Filters
          filters={filters}
          setFilters={setFilters}
          onApply={handleApplyFilters}
        />
        {error && (
          <div className="bg-red-100 p-4 rounded-lg mb-6">
            <h4 className="text-lg font-semibold text-red-800">Error</h4>
            <p className="text-red-700">{error}</p>
          </div>
        )}
        {renderInsights()}
        {loading ? (
          <p className="text-gray-500">Loading...</p>
        ) : (
          <>
            {renderChart()}
            <DataTable
              data={data}
              total={total}
              pageNumber={pageNumber}
              setPageNumber={setPageNumber}
              pageSize={pageSize}
            />
          </>
        )}
      </div>
    </div>
  );
};

export default Dashboard;
