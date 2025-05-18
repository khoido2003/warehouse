import Dashboard from "@/components/Dashboard";
import { notFound } from "next/navigation";

// ✅ Tell Next.js which dynamic routes to pre-render
export async function generateStaticParams() {
  return Array.from({ length: 9 }, (_, i) => ({
    req: String(i + 1),
  }));
}

export default function RequirementPage({
  params,
}: {
  params: { req: string };
}) {
  const requirement = parseInt(params.req, 10);

  if (isNaN(requirement) || requirement < 1 || requirement > 9) {
    notFound();
  }

  return <Dashboard requirement={requirement} />;
}
