import { defineConfig } from "@playwright/test";
import fs from "node:fs";
import path from "node:path";

interface BrowserMatrixContext {
  readonly name: string;
  readonly browser: "chromium";
  readonly viewport: {
    readonly width: number;
    readonly height: number;
  };
  readonly evidenceKind: "emulated";
  readonly testMatch: string;
}

interface BrowserMatrix {
  readonly contexts: readonly BrowserMatrixContext[];
}

const matrix = JSON.parse(fs.readFileSync(
  new URL("./browser-matrix.json", import.meta.url),
  "utf8",
)) as BrowserMatrix;

export default defineConfig({
  testDir: "./tests",
  fullyParallel: true,
  workers: 2,
  retries: process.env.CI ? 1 : 0,
  failOnFlakyTests: Boolean(process.env.CI),
  timeout: 120_000,
  expect: { timeout: 15_000 },
  globalSetup: path.resolve(import.meta.dirname, "support/global-setup.ts"),
  use: {
    baseURL: process.env.HOTJOES_BROWSER_BASE_URL,
    trace: "retain-on-failure",
    screenshot: "only-on-failure",
    video: "off",
  },
  projects: [
    ...matrix.contexts.map((context) => ({
      name: context.name,
      testMatch: context.testMatch,
      use: {
        browserName: context.browser,
        viewport: context.viewport,
      },
    })),
    {
      name: "chromium-headed",
      testIgnore: "**/visual-regression-evidence.spec.ts",
      use: {
        browserName: "chromium",
        viewport: matrix.contexts.find(
          (context) => context.name === "chromium",
        )?.viewport ?? { width: 1440, height: 900 },
      },
    },
    {
      name: "chromium-demo",
      use: {
        viewport: null,
        launchOptions: {
          args: ["--start-maximized", "--start-fullscreen"],
          slowMo: Number(process.env.HOTJOES_BROWSER_DEMO_SLOW_MILLISECONDS ?? "800"),
        },
      },
    },
  ],
  reporter: [
    ["list"],
    ["html", { open: "never" }],
    ["./support/diagnostic-evidence-reporter.mjs"],
  ],
});
