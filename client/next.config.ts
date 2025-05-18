import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  source: "/api/:path*",
  destination: "http://localhost:5164/api/v2/:path*",
};

export default nextConfig;
