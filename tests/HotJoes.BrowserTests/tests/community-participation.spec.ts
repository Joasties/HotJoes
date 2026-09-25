import { expect, test } from "@playwright/test";
import { registerVendor } from "../support/registration-journey";
import {
  communityCounts, composeLogs, contactPreference, eventually, vendorIdFor
} from "../support/runtime-evidence";

test.describe.serial("CON-046 full-stack Community acceptance", () => {
  test("AI-BROWSER-012 Not Now performs no request and creates no Community state", async ({ page }) => {
    const vendor = await registerVendor(page, unique());
    let requests = 0;
    page.on("request", request => {
      if (request.method() === "POST" && request.url().endsWith("/community-participations")) requests++;
    });

    await page.getByRole("button", { name: "Not now" }).click();
    await expect(page.getByRole("heading", { name: "Thank you" })).toBeVisible();
    expect(requests).toBe(0);
    expect(communityCounts(vendorIdFor(vendor.tradingName))).toEqual([0, 0, 0]);
  });

  test("VR-COMMUNITY-001/008/013 join is authoritative, replay-safe and processed once", async ({ page, request }) => {
    const vendor = await registerVendor(page, unique());
    const vendorId = vendorIdFor(vendor.tradingName);

    await page.getByLabel("SMS").check();
    await page.getByLabel(/Keep me involved/).check();
    await page.getByRole("button", { name: "Join the community" }).click();
    await expect(page.getByRole("heading", { name: /Thank you for joining/ })).toBeVisible();

    await eventually(() => expect(communityCounts(vendorId)).toEqual([1, 1, 1]));
    expect(contactPreference(vendorId)).toBe("sms");

    const replay = await request.post("/community-participations", {
      data: { vendorId, contactPreference: "sms" }
    });
    expect(replay.status()).toBe(201);
    expect(communityCounts(vendorId)).toEqual([1, 1, 1]);
  });

  test("AI-BROWSER-006 failure retains selections and permits a safe retry", async ({ page }) => {
    const vendor = await registerVendor(page, unique());
    await page.getByLabel("WhatsApp").check();
    await page.getByLabel(/Keep me involved/).check();
    await page.route("**/community-participations", route => route.fulfill({
      status: 503,
      contentType: "application/problem+json",
      body: JSON.stringify({ code: "communityPersistenceUnavailable" })
    }));

    await page.getByRole("button", { name: "Join the community" }).click();
    await expect(page.getByRole("alert")).toContainText("temporarily unavailable");
    await expect(page.getByLabel("WhatsApp")).toBeChecked();
    await expect(page.getByLabel(/Keep me involved/)).toBeChecked();

    await page.unroute("**/community-participations");
    await page.getByRole("button", { name: "Join the community" }).click();
    await expect(page.getByRole("heading", { name: /Thank you for joining/ })).toBeVisible();
    await eventually(() => expect(communityCounts(vendorIdFor(vendor.tradingName))).toEqual([1, 1, 1]));
  });

  test("AI-BROWSER-005 retained diagnostics exclude contact data and credentials", async () => {
    const logs = composeLogs();
    expect(logs).not.toContain("@example.test");
    expect(logs).not.toContain("07123456789");
    expect(logs).not.toContain(process.env.HOTJOES_POSTGRES_PASSWORD ?? "browser-tests-only");
    expect(logs).not.toContain("serializedEvent");
  });
});

function unique(): string {
  return `${Date.now()}-${Math.random().toString(16).slice(2, 8)}`;
}
