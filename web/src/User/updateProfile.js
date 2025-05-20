document.addEventListener("DOMContentLoaded", () => {
  const updateProfileForm = document.getElementById("updateProfileForm");
  const updateProfileResult = document.getElementById("updateProfileResult");

  // Prepopulate the form with current profile data stored in localStorage.
  document.getElementById("upUsername").value = localStorage.getItem("currentUsername") || "";
  document.getElementById("upEmail").value = localStorage.getItem("currentEmail") || "";
  document.getElementById("upTelephone").value = localStorage.getItem("currentTelephone") || "";
  // Note: We no longer rely solely on localStorage for roles.

  updateProfileForm.addEventListener("submit", async (event) => {
    event.preventDefault();

    // Get the updated values from the form.
    const username = document.getElementById("upUsername").value.trim();
    const email = document.getElementById("upEmail").value.trim();
    const telephoneNumber = document.getElementById("upTelephone").value.trim();

    // Retrieve userId and token from localStorage.
    const userId = localStorage.getItem("userId");
    const token = localStorage.getItem("jwtToken");
    if (!userId || !token) {
      updateProfileResult.innerHTML = `<p class="text-warning">User not logged in.</p>`;
      return;
    }

    // First, fetch the current user data (including roles) from the backend.
    let currentUser;
    try {
      const getUserResponse = await fetch(`https://localhost:7262/api/User/${userId}`, {
        method: "GET",
        headers: {
          "Authorization": `Bearer ${token}`
        }
      });
      if (!getUserResponse.ok) {
        throw new Error("Failed to fetch current user data.");
      }
      currentUser = await getUserResponse.json();
    } catch (error) {
      updateProfileResult.innerHTML = `<p class="text-danger">Error fetching user details: ${error.message}</p>`;
      return;
    }

    // Build the update payload with only the editable fields together with the existing roles.
    const payload = {
      username,
      email,
      telephoneNumber,
      roles: currentUser.roles  // Preserve the existing roles from the fetched user data.
    };

    try {
      const response = await fetch(`https://localhost:7262/api/User/${userId}/profile`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${token}`
        },
        body: JSON.stringify(payload)
      });

      if (!response.ok) {
        const errorText = await response.text();
        updateProfileResult.innerHTML = `<p class="text-danger">Error: ${errorText}</p>`;
        return;
      }

      const updatedUser = await response.json();
      updateProfileResult.innerHTML = `<p class="text-success">Profile updated successfully!</p>`;
      // Optionally update localStorage for fields we display.
      localStorage.setItem("currentUsername", updatedUser.username);
      localStorage.setItem("currentEmail", updatedUser.email);
      localStorage.setItem("currentTelephone", updatedUser.telephoneNumber);
      // Optionally, update the currentRoles in localStorage if needed.
    } catch (error) {
      console.error("Update Profile error:", error);
      updateProfileResult.innerHTML = `<p class="text-danger">Error updating profile: ${error.message}</p>`;
    }
  });
});
