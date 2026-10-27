import { routes } from "./app.routes";

describe("application routes", () => {
  it("AI-WEB-003 keeps registered-Vendor retrieval outside registration", () => {
    expect(routes.map((route) => route.path)).toEqual([
      "registration",
      "registered-vendors/:vendorId",
      "",
      "**",
    ]);

    const retrieval = routes.find(
      (route) => route.path === "registered-vendors/:vendorId",
    );
    expect(retrieval?.loadComponent).toBeTypeOf("function");
  });
});
