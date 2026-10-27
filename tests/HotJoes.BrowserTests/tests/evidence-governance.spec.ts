import { expect, test } from "@playwright/test";
import { readFile } from "node:fs/promises";

test("AI-BROWSER-009 visual and accessibility evidence declares its governed limits", async () => {
  const manifest = JSON.parse(await readFile(
    new URL("../evidence/visual-accessibility-evidence.json", import.meta.url),
    "utf8",
  ));

  expect(manifest).toEqual({
    schemaVersion: 1,
    browserProject: "chromium",
    declaredContexts: [
      { name: "desktop", width: 1440, height: 900 },
      { name: "mobile", width: 390, height: 844 },
    ],
    visualBaseline: {
      maxDiffPixels: 0,
      replacementRequiresExplicitReview: true,
    },
    evidenceLimits: {
      automatedChecksEstablishConformance: false,
      emulationIsPhysicalDeviceEvidence: false,
      manualAssistiveTechnologyReviewRequired: true,
      manualUxReviewRequired: true,
    },
  });
});
