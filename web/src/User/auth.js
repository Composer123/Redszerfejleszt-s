document.addEventListener("DOMContentLoaded", () => {
  const loginForm = document.getElementById("loginForm");
  const registerForm = document.getElementById("registerForm");
  const registerLink = document.getElementById("registerLink");

  if (registerLink) {
    registerLink.addEventListener("click", (event) => {
      event.preventDefault();
      if (loginForm && registerForm) {
        loginForm.style.display = "none";
        registerForm.style.display = "block";
      }
    });
  }

  // Handle Login and Store JWT Token along with User ID
  if (loginForm) {
    loginForm.addEventListener("submit", async (event) => {
      event.preventDefault();
      const email = document.getElementById("email").value;
      const password = document.getElementById("password").value;

      try {
        const response = await fetch("https://localhost:7262/api/User/login", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ email, password })
        });

        if (!response.ok)
          throw new Error(`Invalid credentials - Status: ${response.status}`);

        const data = await response.json();
        // Store the token as before…
        localStorage.setItem("jwtToken", data.token);
        localStorage.setItem("jwtExpireTime", Date.now() + (60 * 60 * 1000)); // 1 hour expiration

        // Store the user ID separately.
        // This assumes your backend returns a "userId" property.
        localStorage.setItem("userId", data.userId);

        console.log("Stored JWT Token:", data.token);
        console.log("Stored User ID:", data.userId);

        alert("Login successful!");
        window.location.href = "../index.html"; // Adjust redirection path as needed.

      } catch (error) {
        console.error("Login error:", error);
        alert(error.message);
      }
    });
  }

  if (registerForm) {
    registerForm.addEventListener("submit", async (event) => {
      event.preventDefault();
      const email = document.getElementById("regEmail").value;
      const username = document.getElementById("regUsername").value;
      // Do NOT convert telephone to an integer; keep it as a string so it can include a plus sign and spaces.
      const telephoneNumber = document.getElementById("regTelephone").value;
      const password = document.getElementById("regPassword").value;

      // Map selected options to an array of objects with the keys matching the backend (roleId and roleName)
      const roles = Array.from(document.getElementById("regRoles").selectedOptions)
        .map(option => ({
          roleId: parseInt(option.value, 10),
          roleName: option.text
        }));

      if (!email || !username || !telephoneNumber || !password || roles.length === 0) {
        alert("All fields are required, including selecting a role!");
        return;
      }

      try {
        const registrationPayload = {
          telephoneNumber, // now a string, e.g. "+36 70 535 6255"
          email,
          username,
          password,
          roles
        };

        console.log("Sending Registration Request:", JSON.stringify(registrationPayload, null, 2));

        const response = await fetch("https://localhost:7262/api/User/register", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(registrationPayload)
        });

        if (!response.ok)
          throw new Error(`Registration failed - Status: ${response.status}`);

        alert("Registration successful! Please log in.");
        registerForm.style.display = "none";
        document.getElementById("loginForm").style.display = "block";

      } catch (error) {
        console.error("Registration error:", error);
        alert(error.message);
      }
    });
  }

});
