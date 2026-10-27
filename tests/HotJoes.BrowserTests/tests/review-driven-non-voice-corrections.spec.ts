import { expect, test, type Page, type TestInfo } from "@playwright/test";
import { createBrowserTestIdentity } from "../support/browser-runtime-lifecycle.mjs";
import { registerVendor } from "../support/registration-journey";

test("F-001 Steps 1–6 expose one aligned circular information control", async ({ page }) => {
  const states = [
    ["/registration/location", "About Vendor Registration"],
    ["/registration/operator", "About Legal Operator Type"],
    ["/registration/business", "About Contact and Business Details"],
    ["/registration/characteristics", "About Trading Characteristics"],
    ["/registration/review", "About reviewing your registration"],
    ["/registration/confirmation", "About joining the HotJoes community"],
  ] as const;

  for (const [path, name] of states) {
    await page.goto(path);
    const information = page.getByRole("button", { name });
    await expect(information).toBeVisible();
    const geometry = await information.evaluate((button) => {
      const style = getComputedStyle(button);
      const bounds = button.getBoundingClientRect();
      return {
        text: button.textContent?.trim(),
        width: bounds.width,
        height: bounds.height,
        radius: style.borderRadius,
        color: style.color,
      };
    });

    expect(geometry.text, `${path} does not duplicate the information symbol embedded in the background`).toBe("");
    expect(Math.abs(geometry.width - geometry.height)).toBeLessThanOrEqual(1);
    expect(geometry.width).toBeLessThanOrEqual(48);
    expect(geometry.radius).toBe("50%");
    expect(geometry.color).not.toBe("rgba(0, 0, 0, 0)");
  }
});

test("F-002 native radio arrows do not navigate until Continue confirms", async ({ page }) => {
  await page.goto("/registration/location");
  const restaurant = page.getByRole("radio", { name: "Restaurant" });
  await restaurant.focus();
  await page.keyboard.press("ArrowRight");
  await expect(page).toHaveURL(/\/registration\/location$/);
  await expect(page.getByRole("radio", { name: "Stall" })).toBeChecked();
  await page.getByRole("button", { name: "Continue" }).click();
  await expect(page).toHaveURL(/\/registration\/operator$/);

  const soleTrader = page.getByRole("radio", { name: "Sole Trader" });
  await soleTrader.focus();
  await page.keyboard.press("ArrowRight");
  await expect(page).toHaveURL(/\/registration\/operator$/);
  await expect(page.getByRole("radio", { name: "General Partnership" }))
    .toBeChecked();
  await page.getByRole("button", { name: "Continue" }).click();
  await expect(page).toHaveURL(/\/registration\/business$/);
});

test("F-006 Step 1 retains near-square selectable cards in the 200-percent equivalent viewport", async ({ page }) => {
  await page.setViewportSize({ width: 800, height: 450 });
  await page.goto("/registration/location");

  for (const name of ["Restaurant", "Stall", "Kitchen"]) {
    const dimensions = await page.getByRole("radio", { name }).evaluate((radio) => {
      const card = radio.closest("label");
      if (!card) throw new Error("Trading Location radio has no containing card.");
      const bounds = card.getBoundingClientRect();
      return { width: bounds.width, height: bounds.height };
    });
    const aspectRatio = dimensions.width / dimensions.height;
    expect(aspectRatio, `${name} remains visually balanced rather than a horizontal strip`)
      .toBeLessThanOrEqual(1.5);
  }
});

test("F-007 Step 3 stacks its existing content panels in the 200-percent equivalent viewport", async ({ page }) => {
  await page.setViewportSize({ width: 800, height: 450 });
  await page.goto("/registration/business");

  const contactBounds = await panelBounds(
    page,
    "Primary Contact",
    ".details-panel",
  );
  const addressBounds = await panelBounds(
    page,
    "Business Address Search",
    ".address-panel",
  );

  expect(addressBounds.y).toBeGreaterThanOrEqual(
    contactBounds.y + contactBounds.height,
  );
  expect(addressBounds.x).toBeCloseTo(contactBounds.x, 0);
});

test("F-008 Steps 4–6 enter with their heading visible at the 200-percent equivalent viewport", async ({ page }, testInfo) => {
  await page.setViewportSize({ width: 800, height: 450 });
  const suffix = identity(testInfo);

  await page.goto("/registration/location");
  await selectAndContinue(page, "Restaurant", "/registration/location");
  await selectAndContinue(page, "Sole Trader", "/registration/operator");

  await page.getByLabel("Contact name").fill(`Browser Contact ${suffix}`);
  await page.getByLabel("Telephone number").fill("07123456789");
  await page.getByLabel("Email address").fill(`browser-${suffix}@example.test`);
  await page.getByLabel("Trading name").fill(`Browser Vendor ${suffix}`);
  await page.getByLabel("Legal operator name").fill(`Browser Operator ${suffix}`);
  await page.getByLabel("Search for the business address").fill("Hot Joes");
  await page.getByRole("button", { name: /Hot Joes, Blackheath Avenue/ }).click();
  await page.getByRole("button", { name: /^Continue/ }).click();
  await expectEnteredAtHeading(page, "Trading Characteristics");

  await expect(page.getByRole("heading", { name: "Required licence types" }))
    .toBeVisible();
  await page.getByRole("button", { name: /Confirm and continue/ }).click();
  await expectEnteredAtHeading(page, "Review your registration");

  await page.getByLabel("I confirm that I am authorised").check();
  await page.getByLabel("I confirm that the information").check();
  await page.getByLabel(/I accept the HotJoes/).check();
  await page.getByRole("button", { name: /Submit Registration/ }).click();
  await expectEnteredAtHeading(page, "Registration submitted!");
});

test.describe("F-005 terminal Community outcomes", () => {
  test("Not Now cannot restore an actionable Community form through Back", async ({ page }, testInfo) => {
    await registerVendor(page, identity(testInfo));
    await page.getByRole("button", { name: "Not now" }).click();
    await expect(page.getByRole("heading", { name: "Thank you" })).toBeVisible();
    await page.goBack();
    await expectNoActionableCommunityForm(page);
  });

  test("successful Join cannot restore an actionable Community form through Back", async ({ page }, testInfo) => {
    await registerVendor(page, identity(testInfo));
    await page.getByLabel("SMS").check();
    await page.getByLabel(/Keep me involved/).check();
    await page.getByRole("button", { name: "Join the community" }).click();
    await expect(page.getByRole("heading", { name: /Thank you for joining/ }))
      .toBeVisible();
    await page.goBack();
    await expectNoActionableCommunityForm(page);
  });
});

async function panelBounds(
  page: Page,
  headingName: string,
  panelSelector: string,
): Promise<{ x: number; y: number; width: number; height: number }> {
  const heading = page.getByRole("heading", { name: headingName });
  await expect(heading).toBeVisible();
  return heading.evaluate((element, selector) => {
    const panel = element.closest(selector);
    if (!panel) throw new Error(`${element.textContent?.trim()} has no ${selector} panel.`);
    const bounds = panel.getBoundingClientRect();
    return {
      x: bounds.x,
      y: bounds.y,
      width: bounds.width,
      height: bounds.height,
    };
  }, panelSelector);
}

async function selectAndContinue(
  page: Page,
  optionName: string,
  currentPath: string,
): Promise<void> {
  await page.getByRole("radio", { name: optionName }).click({ force: true });
  if (new URL(page.url()).pathname === currentPath) {
    await page.getByRole("button", { name: "Continue" }).click();
  }
}

async function expectEnteredAtHeading(page: Page, headingName: string): Promise<void> {
  const heading = page.getByRole("heading", { name: headingName });
  await expect(heading).toBeInViewport();
  const bounds = await heading.boundingBox();
  expect(bounds).not.toBeNull();
  expect(bounds!.y).toBeGreaterThanOrEqual(0);
  expect(bounds!.y).toBeLessThan(225);
}

async function expectNoActionableCommunityForm(page: Page): Promise<void> {
  await expect(page.getByRole("button", { name: "Join the community" }))
    .toHaveCount(0);
  await expect(page.getByLabel(/Keep me involved/)).toHaveCount(0);
}

function identity(testInfo: TestInfo): string {
  return createBrowserTestIdentity({
    runId: process.env.HOTJOES_BROWSER_RUN_ID ?? "local",
    titlePath: testInfo.titlePath,
    workerIndex: testInfo.workerIndex,
    repeatEachIndex: testInfo.repeatEachIndex,
    retryIndex: testInfo.retry,
  });
}
