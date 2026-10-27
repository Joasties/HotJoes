import { TestInfo } from "@playwright/test";

export interface DemonstrationProfile {
  readonly contactName: string;
  readonly tradingName: string;
  readonly legalOperatorName: string;
  readonly email: string;
  readonly website: string;
  readonly description: string;
}

const sourceProfiles = [
  ["Clara Beaumont", "Copper Kettle Foods", "The Gilded Spoon"],
  ["Marcus Vale", "Hearthstone Provisions", "Ember and Oak"],
  ["Evelyn Hart", "Meadowgate Produce", "The Silver Fig"],
  ["Julian Mercer", "Lantern House Foods", "Saffron House"],
  ["Amelia Rowan", "Orchard Crown Foods", "The Velvet Truffle"],
  ["Nathaniel Cross", "Wild Thyme Provisions", "Juniper Table"],
  ["Beatrice Langley", "Golden Field Foods", "The Painted Pear"],
  ["Dominic Ashford", "Stonebridge Pantry", "Salt and Laurel"],
  ["Helena Marlowe", "Willowbrook Produce", "The Amber Plate"],
  ["Sebastian Crane", "Northstar Fine Foods", "Clementine Hall"],
  ["Rosalie Finch", "Honeycomb Provisions", "The Ivory Apron"],
  ["Theodore Wren", "Red Orchard Foods", "Basil and Brass"],
  ["Imogen Fairchild", "Harvest Moon Pantry", "The Marbled Olive"],
  ["Alexander Blythe", "Bluebell Fine Foods", "Cedar and Sage"],
  ["Florence Avery", "Crown Mill Provisions", "The Golden Chanterelle"],
  ["Gabriel Sterling", "Riverstone Foods", "Rosewood Kitchen"],
  ["Arabella Quinn", "Green Lantern Produce", "The Copper Pheasant"],
  ["Frederick Holloway", "Morning Star Pantry", "Thyme and Velvet"],
  ["Penelope March", "Bramble Hill Foods", "The Porcelain Garden"],
  ["Oliver Pembroke", "Maple Crown Provisions", "Laurel and Flame"],
] as const;

const compact = (value: string): string =>
  value.toLowerCase().replace(/[^a-z]/g, "");

export const DEMONSTRATION_CATALOGUE: readonly DemonstrationProfile[] =
  sourceProfiles.map(([contactName, tradingName, legalOperatorName]) => ({
    contactName,
    tradingName,
    legalOperatorName,
    email: `${compact(contactName)}@test.com`,
    website: `https://www.${compact(legalOperatorName)}.com`,
    description:
      `${tradingName}’s most prestigious food establishment is a must visit. ` +
      "Book now through our website.",
  }));

export const DEMONSTRATION_WEEKLY_HOURS = [
  { day: "Monday", isClosed: true, isOpenAllDay: false, startTime: null, endTime: null },
  { day: "Tuesday", isClosed: false, isOpenAllDay: true, startTime: null, endTime: null },
  { day: "Wednesday", isClosed: false, isOpenAllDay: true, startTime: null, endTime: null },
  { day: "Thursday", isClosed: false, isOpenAllDay: true, startTime: null, endTime: null },
  { day: "Friday", isClosed: false, isOpenAllDay: true, startTime: null, endTime: null },
  { day: "Saturday", isClosed: false, isOpenAllDay: false, startTime: "09:00", endTime: "17:00" },
  { day: "Sunday", isClosed: false, isOpenAllDay: false, startTime: "12:00", endTime: "16:00" },
] as const;

export function demonstrationProfile(
  repeatEachIndex: number,
): DemonstrationProfile {
  return DEMONSTRATION_CATALOGUE[
    repeatEachIndex % DEMONSTRATION_CATALOGUE.length
  ];
}

export function demonstrationProfileFor(
  testInfo: TestInfo,
): DemonstrationProfile {
  return demonstrationProfile(testInfo.repeatEachIndex);
}
