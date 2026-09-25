import { expect, Page } from "@playwright/test";

export interface RegisteredVendor {
  readonly tradingName: string;
  readonly email: string;
  readonly telephone: string;
}

export async function registerVendor(page: Page, suffix: string): Promise<RegisteredVendor> {
  const vendor = {
    tradingName: `Browser Vendor ${suffix}`,
    email: `browser-${suffix}@example.test`,
    telephone: "07123456789"
  };

  await page.goto("/registration/start");
  await page.getByRole("button", { name: "Start registration" }).click();
  await page.locator("label.card").filter({ hasText: "Restaurant" }).click();
  await page.locator("label.card").filter({ hasText: "Sole Trader" }).click();

  await page.getByLabel("Search for the business address").fill("Blackheath");
  await page.getByRole("button", { name: /Hot Joes, Blackheath Avenue/ }).click();

  const contactName = `Browser Contact ${suffix}`;
  const legalOperatorName = `Browser Operator ${suffix}`;
  await page.getByLabel("Contact name").fill(contactName);
  await page.getByLabel("Telephone number").fill(vendor.telephone);
  await page.getByLabel("Email address").fill(vendor.email);
  await page.getByLabel("Trading name").fill(vendor.tradingName);
  await page.getByLabel("Legal operator name").fill(legalOperatorName);

  await expect(page.getByLabel("Contact name")).toHaveValue(contactName);
  await expect(page.getByLabel("Telephone number")).toHaveValue(vendor.telephone);
  await expect(page.getByLabel("Email address")).toHaveValue(vendor.email);
  await expect(page.getByLabel("Trading name")).toHaveValue(vendor.tradingName);
  await expect(page.getByLabel("Legal operator name")).toHaveValue(legalOperatorName);
  await page.getByRole("button", { name: /^Continue/ }).click();

  await expect(page.getByRole("heading", { name: "Required licence types" })).toBeVisible();
  await page.getByRole("button", { name: /Confirm and continue/ }).click();
  await page.getByLabel("I confirm that I am authorised").check();
  await page.getByLabel("I confirm that the information").check();
  await page.getByLabel(/I accept the HotJoes/).check();
  await page.getByRole("button", { name: /Submit Registration/ }).click();
  await expect(page.getByRole("heading", { name: "Registration submitted!" })).toBeVisible();
  return vendor;
}
