import { registrationRoutes } from "./registration.routes";

describe("registrationRoutes", () => {
  it("AI-WEB-003 owns registration and separate post-registration outcomes", () => {
    const childPaths = registrationRoutes[0].children?.map(
      (route) => route.path,
    );
    expect(childPaths).toEqual([
      "business",
      "location",
      "operator",
      "characteristics",
      "review",
      "confirmation",
      "community-joined",
      "community-not-joined",
      "",
      "**",
    ]);
  });

  it("returns unknown registration routes to Step 1", () => {
    const fallback = registrationRoutes[0].children?.at(-1);
    expect(fallback).toMatchObject({ path: "**", redirectTo: "location" });
  });
});
