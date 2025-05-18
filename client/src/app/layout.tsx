import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "Warehouse Analysis Dashboard",
  description: "Dashboard for warehouse data analysis with OLAP operations",
};


export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body>
        <div id="app-root">{children}</div>
      </body>
    </html>
  );
}

