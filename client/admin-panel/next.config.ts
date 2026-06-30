import type { NextConfig } from "next"

const apiUrl = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5101/api"
const apiOrigin = apiUrl.replace(/\/api\/?$/, "")

let imageHostname = "localhost"
let imageProtocol: "http" | "https" = "http"
let imagePort: string | undefined = "5101"

try {
  const parsed = new URL(apiOrigin)
  imageHostname = parsed.hostname
  imageProtocol = parsed.protocol === "https:" ? "https" : "http"
  imagePort = parsed.port || undefined
} catch {
  // keep localhost defaults
}

const nextConfig: NextConfig = {
  images: {
    remotePatterns: [
      {
        protocol: imageProtocol,
        hostname: imageHostname,
        ...(imagePort ? { port: imagePort } : {}),
        pathname: "/img/**",
      },
      {
        protocol: "http",
        hostname: "localhost",
        port: "5101",
        pathname: "/img/**",
      },
      {
        protocol: "http",
        hostname: "127.0.0.1",
        port: "5101",
        pathname: "/img/**",
      },
    ],
  },
}

export default nextConfig
