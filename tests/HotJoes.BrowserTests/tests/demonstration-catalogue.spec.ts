import { expect, test } from "@playwright/test";
import {
  DEMONSTRATION_CATALOGUE,
  DEMONSTRATION_WEEKLY_HOURS,
  demonstrationProfile,
} from "../support/demonstration-catalogue";

test("AI-BROWSER-003 demo catalogue contains exactly twenty complete unique fictional profiles", () => {
  expect(DEMONSTRATION_CATALOGUE).toHaveLength(20);

  const contacts = new Set<string>();
  const tradingNames = new Set<string>();
  const legalOperatorNames = new Set<string>();
  const emails = new Set<string>();
  const websites = new Set<string>();

  for (const profile of DEMONSTRATION_CATALOGUE) {
    expect(profile.contactName.trim()).not.toBe("");
    expect(profile.tradingName.trim()).not.toBe("");
    expect(profile.legalOperatorName.trim()).not.toBe("");
    expect(profile.email).toMatch(/^[a-z]+@test\.com$/);
    expect(profile.website).toMatch(/^https:\/\/www\.[a-z]+\.com$/);
    expect(profile.description).toBe(
      `${profile.tradingName}’s most prestigious food establishment is a must visit. Book now through our website.`,
    );

    contacts.add(profile.contactName);
    tradingNames.add(profile.tradingName);
    legalOperatorNames.add(profile.legalOperatorName);
    emails.add(profile.email);
    websites.add(profile.website);
  }

  expect(contacts.size).toBe(20);
  expect(tradingNames.size).toBe(20);
  expect(legalOperatorNames.size).toBe(20);
  expect(emails.size).toBe(20);
  expect(websites.size).toBe(20);
});

test("AI-BROWSER-003 repeated demonstrations cycle sequentially without persistent state", () => {
  for (let index = 0; index < 20; index++) {
    expect(demonstrationProfile(index)).toBe(DEMONSTRATION_CATALOGUE[index]);
  }
  expect(demonstrationProfile(20)).toBe(DEMONSTRATION_CATALOGUE[0]);
  expect(demonstrationProfile(41)).toBe(DEMONSTRATION_CATALOGUE[1]);
});

test("AI-BROWSER-010 demo-only hours retain all seven approved presentation states", () => {
  expect(DEMONSTRATION_WEEKLY_HOURS).toEqual([
    { day: "Monday", isClosed: true, isOpenAllDay: false, startTime: null, endTime: null },
    { day: "Tuesday", isClosed: false, isOpenAllDay: true, startTime: null, endTime: null },
    { day: "Wednesday", isClosed: false, isOpenAllDay: true, startTime: null, endTime: null },
    { day: "Thursday", isClosed: false, isOpenAllDay: true, startTime: null, endTime: null },
    { day: "Friday", isClosed: false, isOpenAllDay: true, startTime: null, endTime: null },
    { day: "Saturday", isClosed: false, isOpenAllDay: false, startTime: "09:00", endTime: "17:00" },
    { day: "Sunday", isClosed: false, isOpenAllDay: false, startTime: "12:00", endTime: "16:00" },
  ]);
});
