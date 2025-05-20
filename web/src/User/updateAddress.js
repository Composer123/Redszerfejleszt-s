document.addEventListener("DOMContentLoaded", () => {
  const updateAddressForm = document.getElementById("updateAddressForm");
  const updateAddressResult = document.getElementById("updateAddressResult");

  updateAddressForm.addEventListener("submit", async (event) => {
    event.preventDefault();

    // Retrieve and trim form values.
    const settlementName = document.getElementById("settlementName").value.trim();
    const postCodeStr = document.getElementById("postCode").value.trim();
    const streetName = document.getElementById("streetName").value.trim();
    const streetTypeValue = document.getElementById("streetType").value.trim();
    const houseNumberStr = document.getElementById("houseNumber").value.trim();
    const stairwayNumberStr = document.getElementById("stairwayNumber").value.trim();
    const floorNumberStr = document.getElementById("floorNumber").value.trim();
    const doorNumberStr = document.getElementById("doorNumber").value.trim();

    // Validate required fields.
    if (!settlementName || !postCodeStr || !streetName || !streetTypeValue || !houseNumberStr) {
      updateAddressResult.innerHTML = `<p class="text-warning">Please fill all required fields.</p>`;
      return;
    }

    // Convert numeric inputs.
    const postCode = parseInt(postCodeStr, 10);
    const houseNumber = parseInt(houseNumberStr, 10);
    const streetType = parseInt(streetTypeValue, 10);
    const stairwayNumber = stairwayNumberStr ? parseInt(stairwayNumberStr, 10) : null;
    const floorNumber = floorNumberStr ? parseInt(floorNumberStr, 10) : null;
    const doorNumber = doorNumberStr ? parseInt(doorNumberStr, 10) : null;

    // Build the payload to match your SimpleAddressDTO.
    // Settlement is sent as an object matching SettlementDTO.
    const addressPayload = {
      Settlement: {
        SettlementName: settlementName,
        PostCode: postCode
      },
      StreetName: streetName,
      StreetType: streetType, // Now a number, matching the enum type.
      HouseNumber: houseNumber,
      StairwayNumber: stairwayNumber,
      FloorNumber: floorNumber,
      DoorNumber: doorNumber
    };

    // Retrieve userId and token from localStorage.
    const userId = localStorage.getItem("userId");
    const token = localStorage.getItem("jwtToken");
    if (!userId || !token) {
      updateAddressResult.innerHTML = `<p class="text-warning">User not logged in.</p>`;
      return;
    }

    try {
      const response = await fetch(`https://localhost:7262/api/User/${userId}/address`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${token}`
        },
        body: JSON.stringify(addressPayload)
      });

      if (!response.ok) {
        const errorText = await response.text();
        updateAddressResult.innerHTML = `<p class="text-danger">Error updating address: ${errorText}</p>`;
        return;
      }

      const updatedUser = await response.json();
      updateAddressResult.innerHTML = `<p class="text-success">Address updated successfully!</p>`;
      // Optionally update localStorage or the UI with new address info.
    } catch (error) {
      console.error("Error updating address:", error);
      updateAddressResult.innerHTML = `<p class="text-danger">Error updating address: ${error.message}</p>`;
    }
  });
});
