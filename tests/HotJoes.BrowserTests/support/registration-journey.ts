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
    telephone: "07123456789",
  };

  await page.goto("/registration/location");
  const restaurant = page.getByRole("radio", { name: /Restaurant/ });
  await restaurant.check({ force: true });
  await expect(restaurant).toBeChecked();
  const locationContinue = page.getByRole("button", { name: "Continue" });
  await expect(locationContinue).toBeEnabled();
  await locationContinue.click();
  await expect(page).toHaveURL(/\/registration\/operator$/);

  const soleTrader = page.getByRole("radio", { name: /Sole Trader/ });
  await soleTrader.check({ force: true });
  await expect(soleTrader).toBeChecked();
  const operatorContinue = page.getByRole("button", { name: "Continue" });
  await expect(operatorContinue).toBeEnabled();
  await operatorContinue.click();
  await expect(page).toHaveURL(/\/registration\/business$/);

  const addressResponse = page.waitForResponse((response) =>
    response.request().method() === "GET" &&
    new URL(response.url()).pathname === "/address-search" &&
    new URL(response.url()).searchParams.get("query") === "Hot Joes");
  const addressQuery = page.getByLabel("Search for the business address");
  await addressQuery.click();
  await addressQuery.pressSequentially("Hot Joes");
  await expect(addressQuery).toHaveValue("Hot Joes");
  expect((await addressResponse).status()).toBe(200);
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
