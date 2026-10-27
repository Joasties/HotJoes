import { expect, test } from "@playwright/test";

test.describe("AI-BROWSER-009 governed visual baseline", () => {
  test.use({ viewport: { width: 1440, height: 900 } });

  test("Step 1 matches its explicitly reviewed desktop baseline", async ({ page }) => {
    await page.goto("/registration/location");
    await expect(page.getByRole("heading", { name: "Choose your trading location" }))
      .toBeVisible();

    await expect(page).toHaveScreenshot(
      "registration-step-1-desktop.png",
      {
        animations: "disabled",
        fullPage: true,
        maxDiffPixels: 0,
      },
    );
  });
});
