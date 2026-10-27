import { expect, Page, TestInfo } from "@playwright/test";
import { createBrowserTestIdentity } from "./browser-runtime-lifecycle.mjs";
import { DEMONSTRATION_WEEKLY_HOURS, demonstrationProfileFor } from "./demonstration-catalogue";

export interface RepresentativeVendor {
  readonly vendorId: string;
  readonly tradingName: string;
  readonly legalOperatorName: string;
  readonly contactName: string;
  readonly email: string;
  readonly telephone: string;
}

export type RepresentativeJourneyState = "trading-location" | "legal-operator" |
  "business-identity" | "trading-characteristics" | "registration-review" |
  "registration-confirmation";
export type RepresentativeJourneyObserver = (state: RepresentativeJourneyState, page: Page) => Promise<void>;

export function demonstrationIdentity(testInfo: TestInfo): string {
  return createBrowserTestIdentity({
    runId: process.env.HOTJOES_BROWSER_RUN_ID ?? "local",
    titlePath: testInfo.titlePath,
    workerIndex: testInfo.workerIndex,
    repeatEachIndex: testInfo.repeatEachIndex,
    retryIndex: testInfo.retry,
  });
}

export function isInteractiveDemonstration(): boolean {
  return process.env.HOTJOES_BROWSER_DEMONSTRATION === "true";
}

export async function registerRepresentativeVendor(
  page: Page,
  identity: string,
  observe: RepresentativeJourneyObserver = async () => undefined,
  testInfo?: TestInfo,
): Promise<RepresentativeVendor> {
  const interactive = isInteractiveDemonstration();
  const profile = interactive && testInfo ? demonstrationProfileFor(testInfo) : {
    tradingName: `Browser Vendor ${identity}`,
    legalOperatorName: `Browser Operator ${identity}`,
    contactName: `Browser Contact ${identity}`,
    email: `browser-${identity}@example.test`,
    website: "",
    description: "",
  };
  const vendor = { ...profile, telephone: "07123456789" };

  await page.goto("/registration/location");
  await observe("trading-location", page);
  if (interactive) {
    await waitForDemonstrationStart(page);
    await showInformation(page, "About Vendor Registration");
    await presentOptions(page, ["Restaurant", "Stall", "Kitchen"]);
  }
  const restaurant = page.getByRole("radio", { name: /Restaurant/ });
  await restaurant.check({ force: true });
  await expect(restaurant).toBeChecked();
  const locationContinue = page.getByRole("button", { name: "Continue" });
  await expect(locationContinue).toBeEnabled();
  await locationContinue.click();
  await expect(page).toHaveURL(/\/registration\/operator$/);

  await observe("legal-operator", page);
  if (interactive) {
    await showInformation(page, "About Legal Operator Type");
    await presentOptions(page, ["Sole Trader", "General Partnership", "Limited Company",
      "Limited Liability Partnership", "Charitable Community Group",
      "Charitable Incorporated Organisation"]);
  }
  const soleTrader = page.getByRole("radio", { name: /Sole Trader/ });
  await soleTrader.check({ force: true });
  await expect(soleTrader).toBeChecked();
  const operatorContinue = page.getByRole("button", { name: "Continue" });
  await expect(operatorContinue).toBeEnabled();
  await operatorContinue.click();
  await expect(page).toHaveURL(/\/registration\/business$/);

  if (interactive) await showInformation(page, "About Contact and Business Details");
  const fillBusinessIdentity = async (): Promise<void> => {
    await page.getByLabel("Contact name").fill(vendor.contactName);
    await page.getByLabel("Telephone number").fill(vendor.telephone);
    await page.getByLabel("Email address").fill(vendor.email);
    await page.getByLabel("Trading name").fill(vendor.tradingName);
    await page.getByLabel("Legal operator name").fill(vendor.legalOperatorName);
  };

  if (interactive) await fillBusinessIdentity();
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
  if (!interactive) await fillBusinessIdentity();

  await expect(page.getByLabel("Contact name")).toHaveValue(vendor.contactName);
  await expect(page.getByLabel("Telephone number")).toHaveValue(vendor.telephone);
  await expect(page.getByLabel("Email address")).toHaveValue(vendor.email);
  await expect(page.getByLabel("Trading name")).toHaveValue(vendor.tradingName);
  await expect(page.getByLabel("Legal operator name"))
    .toHaveValue(vendor.legalOperatorName);
  await expect(page.getByText("Hot Joes", { exact: true })).toBeVisible();
  await expect(page.getByText("Blackheath Avenue", { exact: true })).toBeVisible();
  await observe("business-identity", page);
  await page.getByRole("button", { name: /^Continue/ }).click();
  await expect(page).toHaveURL(/\/registration\/characteristics$/);
  await expect(page.getByRole("button", { name: "Customise Hours" })).toBeVisible();

  if (interactive) {
    await showInformation(page, "About Trading Characteristics");
    await configureDemonstrationWeeklyHours(page);
    await addOptionalProfileInformation(page, vendor.website, vendor.description);
  } else {
    await configureMixedWeeklyHours(page);
  }
  await expect(page.getByRole("heading", { name: "Required licence types" })).toBeVisible();
  await observe("trading-characteristics", page);
  await page.getByRole("button", { name: /Confirm and continue/ }).click();

  if (interactive) {
    await showInformation(page, "About reviewing your registration");
    await expect(page.getByText(vendor.tradingName, { exact: true })).toBeVisible();
    await expect(page.getByText(vendor.website, { exact: true })).toBeVisible();
  }
  await observe("registration-review", page);
  await page.getByLabel("I confirm that I am authorised").check();
  await page.getByLabel("I confirm that the information").check();
  await page.getByLabel(/I accept the HotJoes/).check();

  const registrationResponse = page.waitForResponse((response) =>
    response.request().method() === "POST" &&
    new URL(response.url()).pathname === "/vendors" && response.status() === 201);
  await page.getByRole("button", { name: /Submit Registration/ }).click();
  const result = await (await registrationResponse).json() as {
    readonly vendorId: string;
    readonly vendorState: string;
  };
  expect(result.vendorId).toMatch(/^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/);
  await expect(page.getByRole("heading", { name: "Registration submitted!" })).toBeVisible();
  await observe("registration-confirmation", page);

  if (interactive) {
    await showInformation(page, "About joining the HotJoes community");
    await page.getByRole("radio", { name: "SMS" }).check();
    await page.getByLabel("Keep me involved").check();
    await page.getByRole("button", { name: "Join the community" }).click();
    await expect(page.getByRole("heading", { name: "Thank you for joining the HotJoes community" })).toBeVisible();
  }
  return { ...vendor, vendorId: result.vendorId };
}

export async function showInformation(page: Page, accessibleName: string): Promise<void> {
  const information = page.getByRole("button", { name: accessibleName });
  await information.focus();
  await expect(information).toBeFocused();
}

export async function presentOptions(page: Page, accessibleNames: readonly string[]): Promise<void> {
  for (const accessibleName of accessibleNames) {
    const option = page.getByRole("radio", { name: accessibleName });
    await option.focus();
    await page.getByText(accessibleName, { exact: true }).hover();
  }
}

export async function waitForDemonstrationEnd(page: Page): Promise<void> {
  if (!isInteractiveDemonstration()) return;
  await addDemonstrationControl(page, "Demonstration complete", "End demonstration", "hotJoesDemonstrationComplete");
}

async function waitForDemonstrationStart(page: Page): Promise<void> {
  await addDemonstrationControl(page, "Ready to demonstrate Vendor Registration", "Start registration", "hotJoesDemonstrationStarted");
}

async function addDemonstrationControl(
  page: Page, messageText: string, buttonText: string, completionKey: string,
): Promise<void> {
  await page.evaluate(({ messageText, buttonText, completionKey }) => {
    const panel = document.createElement("aside");
    panel.setAttribute("aria-label", "Demonstration controls");
    Object.assign(panel.style, {
      position: "fixed", right: "24px", bottom: "24px", zIndex: "2147483647",
      display: "flex", alignItems: "center", gap: "16px", padding: "16px 20px",
      border: "2px solid #ff8a00", borderRadius: "16px", color: "#ffffff",
      background: "#090806", boxShadow: "0 8px 30px rgba(0, 0, 0, .55)",
      font: "600 18px system-ui, sans-serif",
    });
    const message = document.createElement("span");
    message.textContent = messageText;
    const button = document.createElement("button");
    button.type = "button";
    button.textContent = buttonText;
    Object.assign(button.style, {
      padding: "12px 18px", border: "0", borderRadius: "10px", color: "#090806",
      background: "#ff8a00", font: "700 18px system-ui, sans-serif", cursor: "pointer",
    });
    button.addEventListener("click", () => {
      document.documentElement.dataset[completionKey] = "true";
      panel.remove();
    });
    panel.append(message, button);
    document.body.append(panel);
    button.focus();
  }, { messageText, buttonText, completionKey });
  await page.waitForFunction((key) => document.documentElement.dataset[key] === "true", completionKey, { timeout: 0 });
}

async function configureDemonstrationWeeklyHours(page: Page): Promise<void> {
  await page.getByRole("button", { name: "Customise Hours" }).click();
  const dialog = page.getByRole("dialog", { name: "Customise Hours" });
  for (const hours of DEMONSTRATION_WEEKLY_HOURS) {
    if (hours.isClosed) await dialog.getByLabel(`${hours.day} closed`).check();
    else if (hours.isOpenAllDay) await dialog.getByLabel(`${hours.day} open all day`).check();
    else {
      await dialog.getByLabel(`${hours.day} start time`).fill(hours.startTime!);
      await dialog.getByLabel(`${hours.day} end time`).fill(hours.endTime!);
    }
  }
  await dialog.getByRole("button", { name: "Save hours" }).click();
}

async function addOptionalProfileInformation(page: Page, website: string, description: string): Promise<void> {
  await page.getByRole("button", { name: "Add Website" }).click();
  const websiteDialog = page.getByRole("dialog", { name: "Add Website" });
  await websiteDialog.getByLabel("Website address").fill(website);
  await websiteDialog.getByRole("button", { name: "Save website" }).click();
  await page.getByRole("button", { name: "Add Business Description" }).click();
  const descriptionDialog = page.getByRole("dialog", { name: "Add Business Description" });
  await descriptionDialog.getByLabel("Business description").fill(description);
  await descriptionDialog.getByRole("button", { name: "Save description" }).click();
}

async function configureMixedWeeklyHours(page: Page): Promise<void> {
  await page.getByRole("button", { name: "Customise Hours" }).click();
  const dialog = page.getByRole("dialog", { name: "Customise Hours" });
  await dialog.getByLabel("Monday closed").check();
  await dialog.getByLabel("Tuesday start time").fill("09:00");
  await dialog.getByLabel("Tuesday end time").fill("17:00");
  await dialog.getByLabel("Wednesday start time").fill("10:00");
  await dialog.getByLabel("Wednesday end time").fill("18:00");
  await dialog.getByLabel("Thursday start time").fill("11:00");
  await dialog.getByLabel("Thursday end time").fill("19:00");
  await dialog.getByLabel("Friday open all day").check();
  await dialog.getByLabel("Saturday open all day").check();
  await dialog.getByLabel("Sunday closed").check();
  await dialog.getByRole("button", { name: "Save hours" }).click();
}
