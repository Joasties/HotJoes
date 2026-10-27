import { expect, test } from "@playwright/test";
import {
  demonstrationIdentity,
  isInteractiveDemonstration,
  registerRepresentativeVendor,
  waitForDemonstrationEnd,
} from "../support/representative-demonstration";
import {
  complianceReceiptEvidence,
  eventually,
  sha256,
  vendorEventEvidence,
} from "../support/runtime-evidence";

test("AI-BROWSER-004/010 demonstrates authoritative registration, retrieval and Compliance receipt", async ({ page }, testInfo) => {
  if (isInteractiveDemonstration()) testInfo.setTimeout(0);

  const vendor = await registerRepresentativeVendor(
    page,
    demonstrationIdentity(testInfo),
    async () => undefined,
    testInfo,
  );

  if (!isInteractiveDemonstration()) {
    await page.goto(`/registered-vendors/${vendor.vendorId}`);
    await expect(page.getByRole("heading", { name: "Registered Vendor details" }))
      .toBeVisible();

    const details = page.getByRole("main");
    await expect(details.getByText(vendor.vendorId, { exact: true })).toBeVisible();
    await expect(details.getByText(vendor.tradingName, { exact: true })).toBeVisible();
    await expect(details.getByText(vendor.legalOperatorName, { exact: true })).toBeVisible();
    await expect(details.getByText(vendor.email, { exact: true })).toBeVisible();
    await expect(details.getByText("Blackheath Avenue", { exact: true })).toBeVisible();

    const hours = details.getByRole("region", { name: "Weekly opening hours" });
    await expect(hours).toContainText("Monday");
    await expect(hours).toContainText("Closed");
    await expect(hours).toContainText("Tuesday");
    await expect(hours).toContainText("09:00 to 17:00");
    await expect(hours).toContainText("Wednesday");
    await expect(hours).toContainText("10:00 to 18:00");
    await expect(hours).toContainText("Thursday");
    await expect(hours).toContainText("11:00 to 19:00");
    await expect(hours).toContainText("Friday");
    await expect(hours).toContainText("Open all day");
    await expect(hours).toContainText("Saturday");
    await expect(hours).toContainText("Sunday");
  }

  const event = vendorEventEvidence(vendor.vendorId);
  const envelope = JSON.parse(Buffer.from(event.serializedEvent).toString("utf8")) as {
    readonly eventId: string;
    readonly eventVersion: number;
    readonly payload: {
      readonly vendorId: string;
      readonly tradingCharacteristics: {
        readonly weeklyOpeningHours: {
          readonly days: readonly {
            readonly day: string;
            readonly isClosed: boolean;
            readonly isOpenAllDay: boolean;
            readonly startTime: string | null;
            readonly endTime: string | null;
          }[];
        };
      };
    };
  };
  expect(envelope.eventId).toBe(event.eventId);
  expect(envelope.eventVersion).toBe(1);
  expect(envelope.payload.vendorId).toBe(vendor.vendorId);
  const canonicalHours = [
    { day: "monday", isClosed: true, isOpenAllDay: false, startTime: null, endTime: null },
    { day: "tuesday", isClosed: false, isOpenAllDay: false, startTime: "09:00:00", endTime: "17:00:00" },
    { day: "wednesday", isClosed: false, isOpenAllDay: false, startTime: "10:00:00", endTime: "18:00:00" },
    { day: "thursday", isClosed: false, isOpenAllDay: false, startTime: "11:00:00", endTime: "19:00:00" },
    { day: "friday", isClosed: false, isOpenAllDay: true, startTime: null, endTime: null },
    { day: "saturday", isClosed: false, isOpenAllDay: true, startTime: null, endTime: null },
    { day: "sunday", isClosed: true, isOpenAllDay: false, startTime: null, endTime: null },
  ];
  const demonstrationHours = [
    { day: "monday", isClosed: true, isOpenAllDay: false, startTime: null, endTime: null },
    { day: "tuesday", isClosed: false, isOpenAllDay: true, startTime: null, endTime: null },
    { day: "wednesday", isClosed: false, isOpenAllDay: true, startTime: null, endTime: null },
    { day: "thursday", isClosed: false, isOpenAllDay: true, startTime: null, endTime: null },
    { day: "friday", isClosed: false, isOpenAllDay: true, startTime: null, endTime: null },
    { day: "saturday", isClosed: false, isOpenAllDay: false, startTime: "09:00:00", endTime: "17:00:00" },
    { day: "sunday", isClosed: false, isOpenAllDay: false, startTime: "12:00:00", endTime: "16:00:00" },
  ];
  expect(envelope.payload.tradingCharacteristics.weeklyOpeningHours.days)
    .toEqual(isInteractiveDemonstration() ? demonstrationHours : canonicalHours);

  await eventually(() => {
    const receipt = complianceReceiptEvidence(event.eventId);
    expect(receipt.eventId).toBe(event.eventId);
    expect(receipt.eventType).toBe("VendorRegistered");
    expect(receipt.eventVersion).toBe(1);
    expect(receipt.serializedEventSha256).toBe(sha256(event.serializedEvent));
  });

  await waitForDemonstrationEnd(page);
});
