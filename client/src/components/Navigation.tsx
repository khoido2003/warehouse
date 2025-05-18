"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";

const Navigation = () => {
  const pathname = usePathname();
  const requirements = [
    "Stores & Products",
    "Orders & Customers",
    "Stores with Ordered Products",
    "Office Addresses",
    "Order Details",
    "Customer Cities",
    "Inventory Levels",
    "Order Items",
    "Customer Types",
  ];

  const handleLinkClick = (req: string, index: number) => {
    console.log(`Navigating to /requirement${index + 1} from ${pathname}`);
  };

  return (
    <nav className="bg-blue-600 text-white p-4 shadow-md">
      <div className="container mx-auto flex justify-between items-center">
        <h1 className="text-xl font-bold">Warehouse Analysis Dashboard</h1>
        <div className="flex space-x-4">
          {requirements.map((req, index) => {
            const href = index === 0 ? "/" : `/requirement/${index + 1}`;
            const isActive =
              pathname === href || (index === 0 && pathname === "/");
            return (
              <Link
                key={index}
                href={href}
                onClick={() => handleLinkClick(req, index)}
                className={`px-3 py-2 rounded transition ${
                  isActive ? "bg-blue-700" : "hover:bg-blue-700"
                }`}
              >
                {req}
              </Link>
            );
          })}
        </div>
      </div>
    </nav>
  );
};

export default Navigation;
