import { NextResponse } from "next/server";
import axios from "axios";

export async function GET(request: Request) {
  const { searchParams, pathname } = new URL(request.url);
  const apiPath = pathname.replace("/api/", "");

  try {
    const response = await axios.get(`http://localhost:5164/api/${apiPath}`, {
      params: Object.fromEntries(searchParams),
    });
    return NextResponse.json(response.data);
  } catch (error: any) {
    console.error("API Proxy Error:", {
      apiPath,
      params: Object.fromEntries(searchParams),
      error: error.message,
      response: error.response?.data,
    });
    return NextResponse.json(
      {
        error: "Failed to fetch data",
        details: error.message,
        status: error.response?.status,
      },
      { status: error.response?.status || 500 },
    );
  }
}
