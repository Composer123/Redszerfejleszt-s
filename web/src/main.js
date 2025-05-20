// Global helper functions
function parseJwt(token) {
  const base64Url = token.split('.')[1];
  if (!base64Url) return null;
  const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
  const jsonPayload = decodeURIComponent(
    atob(base64)
      .split('')
      .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
      .join('')
  );
  return JSON.parse(jsonPayload);
}

function getStoredUserId() {
  let userId = localStorage.getItem("userId");
  if (!userId || userId === "undefined") {
    const token = localStorage.getItem("jwtToken");
    if (token) {
      const decoded = parseJwt(token);
      console.log("Decoded token:", decoded);  // Debug
      // Adjust the claim name if necessary.
      userId = decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
      localStorage.setItem("userId", userId);
    }
  }
  return userId;
}

// Helper utility functions using Bootstrap's d-none (if not already defined)
function showElement(el, displayStyle = "block") {
  if (el) {
    el.classList.remove("d-none");
    el.style.display = displayStyle;
  }
}
function hideElement(el) {
  if (el) {
    el.classList.add("d-none");
  }
}

// Document ready
document.addEventListener("DOMContentLoaded", () => {
  // Cache navigation & content containers.
  const productsLink = document.getElementById("productsLink");
  const productList = document.getElementById("productList");
  const userLink = document.getElementById("userLink");
  const warehouseLogo = document.querySelector(".navbar-brand");
  const postReviewButton = document.getElementById("postReviewButton");
  const getUndeliveredOrdersByUserIdLink = document.getElementById("getUndeliveredOrdersByUserId");
  const getOrdersByUserIdLink = document.getElementById("getOrdersByUserId");

  // Cache review-related containers.
  const ordersContainer = document.getElementById("ordersContainer");
  const reviewFormContainer = document.getElementById("reviewFormContainer");
  // Cache the welcome screen.
  const welcomeScreen = document.getElementById("welcomeScreen");

  console.log("JWT Token:", localStorage.getItem("jwtToken"));
  console.log("UserID from helper:", getStoredUserId());
  const userId = getStoredUserId();
  console.log("UserID from helper:", userId);


  // Initialize views.
  showElement(welcomeScreen, "flex");
  hideElement(productList);
  hideElement(ordersContainer);
  hideElement(reviewFormContainer);

  // Assume these functions exist elsewhere.
  updateUserLink();
  updateRoleMenus();

  // --- Helper to clear review-related views ---
  function clearReviewViews() {
    if (ordersContainer) {
      ordersContainer.innerHTML = "";
      hideElement(ordersContainer);
    }
    if (reviewFormContainer) {
      reviewFormContainer.innerHTML = "";
      hideElement(reviewFormContainer);
    }
  }

  // Helper functions to remove or restore the welcome screen.
  function removeWelcomeScreen() {
    const welcomeScreen = document.getElementById("welcomeScreen");
    if (welcomeScreen) {
      hideElement(welcomeScreen);
    }
  }

  // Utility: Restore the welcome screen.
  function restoreWelcomeScreen() {
    const existingWelcome = document.getElementById("welcomeScreen");
    if (existingWelcome) existingWelcome.remove();
    const restoredWelcome = document.createElement("div");
    restoredWelcome.id = "welcomeScreen";
    restoredWelcome.className = "d-flex flex-column align-items-center justify-content-center vh-100 text-center bg-dark text-white";
    restoredWelcome.innerHTML = `
      <h1 class="fw-bolder display-1">Welcome to <span class="text-warning">Warehouse</span></h1>
      <p class="fs-3">Your logistics, your control.</p>
  `;
    document.body.prepend(restoredWelcome);
  }

  // --- Review Workflow Functions ---

  // Function to submit the review to your backend.
  async function submitReview(event) {
    event.preventDefault();
    const starRatingInput = document.getElementById("starRating");
    const feedbackInput = document.getElementById("feedbackText");
    const orderIdInput = document.getElementById("orderId");
    const starRating = starRatingInput ? starRatingInput.value : 0;
    const feedbackText = feedbackInput ? feedbackInput.value : "";
    const orderId = orderIdInput ? orderIdInput.value : null;

    const reviewData = {
      starRating: Number(starRating),
      feedbackText: feedbackText,
      orderId: orderId // Ensure your backend expects this property.
    };

    try {
      const token = localStorage.getItem("jwtToken");
      const response = await fetch("https://localhost:7262/api/Feedback/reviews", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${token}`
        },
        body: JSON.stringify(reviewData)
      });
      if (!response.ok) {
        throw new Error(`Failed to submit review: ${response.status}`);
      }
      const result = await response.json();
      console.log("Review submitted successfully:", result);
      reviewFormContainer.innerHTML = `<p class="text-success">Your review has been submitted successfully!</p>`;
    } catch (error) {
      console.error("Error submitting review:", error);
      reviewFormContainer.innerHTML = `<p class="text-danger">Error submitting review: ${error.message}</p>`;
    }
  }

  // Function to display the review form for a given order.
  function showReviewForm(orderId) {
    if (!reviewFormContainer) return;
    reviewFormContainer.innerHTML = `
      <h3 class="text-white">Submit a Review for Order ${orderId}</h3>
      <form id="reviewForm">
        <div class="mb-3">
          <label for="starRating" class="form-label">Star Rating (0-5):</label>
          <input type="number" id="starRating" name="starRating" class="form-control" min="0" max="5" required>
        </div>
        <div class="mb-3">
          <label for="feedbackText" class="form-label">Feedback:</label>
          <textarea id="feedbackText" name="feedbackText" class="form-control" rows="4" placeholder="Enter your review here..."></textarea>
        </div>
        <!-- Hidden input to capture the order ID -->
        <input type="hidden" id="orderId" value="${orderId}">
        <button type="submit" class="btn btn-warning">Submit Review</button>
      </form>
    `;
    showElement(reviewFormContainer, "block");

    const reviewForm = document.getElementById("reviewForm");
    if (reviewForm) {
      reviewForm.addEventListener("submit", submitReview);
    }
  }

  // Function to fetch and display the user's orders in a grayish table.
  async function showOrdersListForReview() {
    if (!ordersContainer) return;
    ordersContainer.innerHTML = "<p>Loading your orders...</p>";
    const token = localStorage.getItem("jwtToken");

    try {
      const response = await fetch(`https://localhost:7262/api/Order/user/${userId}`, {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${token}`
        }
      });
      if (!response.ok) {
        throw new Error(`Unable to fetch orders: ${response.status}`);
      }
      const orders = await response.json();
      if (orders.length === 0) {
        ordersContainer.innerHTML = `<p>No orders found for your account.</p>`;
        return;
      }
      let tableHTML = `<table class="table table-dark table-striped mt-3">
                          <thead>
                            <tr>
                              <th>Order ID</th>
                              <th>Order Date</th>
                              <th>Status</th>
                            </tr>
                          </thead>
                          <tbody>`;
      orders.forEach(order => {
        tableHTML += `<tr data-orderid="${order.orderId}" class="order-row" style="cursor:pointer;">
                        <td>${order.orderId}</td>
                        <td>${order.orderDate}</td>
                        <td>${order.status}</td>
                      </tr>`;
      });
      tableHTML += `</tbody></table>`;
      ordersContainer.innerHTML = tableHTML;
      showElement(ordersContainer, "block");
    } catch (error) {
      ordersContainer.innerHTML = `<p class="text-danger">Error: ${error.message}</p>`;
    }
  }

  // Use event delegation on the ordersContainer to handle clicks on order rows.
  if (ordersContainer) {
    ordersContainer.addEventListener("click", (event) => {
      let targetElement = event.target;
      while (targetElement && targetElement !== ordersContainer) {
        if (targetElement.classList && targetElement.classList.contains("order-row")) {
          const orderId = targetElement.getAttribute("data-orderid");
          console.log("Order row clicked:", orderId);
          showReviewForm(orderId);
          break;
        }
        targetElement = targetElement.parentNode;
      }
    });
  }

  // --- Navigation Handlers ---

  // Products: Clicking "Products" loads the products view.
  if (productsLink) {
    productsLink.addEventListener("click", async (event) => {
      event.preventDefault();
      removeWelcomeScreen();
      clearReviewViews();
      hideElement(productList);
      const token = localStorage.getItem("jwtToken");
      try {
        const response = await fetch("https://localhost:7262/api/Product", {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
          }
        });
        if (!response.ok) {
          throw new Error("Unauthorized: You may not have permission to view products.");
        }
        const data = await response.json();
        console.log("Fetched Data:", data);
        if (data.length === 0) {
          productList.innerHTML = `<p class="text-warning text-center mt-3">No products available.</p>`;
          showElement(productList);
          return;
        }
        let output = `<table class="table table-dark table-striped mt-3">
                        <thead>
                          <tr>
                            <th>Product ID</th>
                            <th>Name</th>
                            <th>Price</th>
                            <th>Type</th>
                            <th>Max Quantity</th>
                          </tr>
                        </thead>
                        <tbody>`;
        data.forEach(product => {
          output += `<tr>
                      <td>${product.productId}</td>
                      <td>${product.name}</td>
                      <td>${product.price}</td>
                      <td>${product.type}</td>
                      <td>${product.maxQuantityPerBlock}</td>
                    </tr>`;
        });
        output += `</tbody></table>`;
        productList.innerHTML = output;
        showElement(productList);
      } catch (error) {
        console.error("Error loading products:", error);
        productList.innerHTML = `<p class="text-danger text-center mt-3">${error.message}</p>`;
        showElement(productList);
      }
    });
  }

  // Warehouse logo event handler
  if (warehouseLogo) {
    warehouseLogo.addEventListener("click", (event) => {
      event.preventDefault();
      // Clear any previous content.
      const productList = document.getElementById("productList");
      if (productList) {
        productList.innerHTML = "";
        productList.style.display = "none";
      }
      // Call your clearReviewViews() helper (ensure only one definition exists).
      clearReviewViews();
      // Now, forcefully restore the welcome screen.
      const welcomeScreen = document.getElementById("welcomeScreen");
      if (welcomeScreen) {
        // Remove any hiding classes and set the inline style to reveal it.
        welcomeScreen.classList.remove("d-none");
        welcomeScreen.style.display = "flex";
      }
      console.log("Warehouse logo clicked; welcome screen restored.");
    });
  }

  // Write a Review: Clicking "Write a Review" initiates the review workflow.
  if (postReviewButton) {
    postReviewButton.addEventListener("click", (event) => {
      event.preventDefault();
      removeWelcomeScreen();
      productList.innerHTML = "";
      hideElement(productList);
      clearReviewViews();
      showElement(ordersContainer);
      showOrdersListForReview();
    });
  }

  // ------------------------------
  // Undelivered Orders and All Orders handlers:
  // ------------------------------
  if (getUndeliveredOrdersByUserIdLink) {
    getUndeliveredOrdersByUserIdLink.addEventListener("click", async (event) => {
      event.preventDefault();
      console.log("Undelivered Orders link clicked");
      removeWelcomeScreen();
      clearReviewViews();
      const userId = getStoredUserId();
      if (!userId) {
        productList.innerHTML = `<p class="text-warning">User ID is not available. Please log in again.</p>`;
        showElement(productList);
        return;
      }
      productList.innerHTML = `
        <div id="undeliveredOrdersByUserIdContainer" class="container mt-3">
          <h3>Undelivered Orders for User ID: ${userId}</h3>
          <div id="undeliveredOrdersResult" class="mt-3"></div>
        </div>
      `;
      showElement(productList);
      const resultDiv = document.getElementById("undeliveredOrdersResult");
      const token = localStorage.getItem("jwtToken");
      try {
        const response = await fetch(`https://localhost:7262/api/Order/delivery/user/${userId}`, {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
          }
        });
        if (!response.ok) {
          throw new Error(`Error: ${response.status}`);
        }
        const orders = await response.json();
        if (orders.length === 0) {
          resultDiv.innerHTML = `<p class="text-warning">No undelivered orders found for your account.</p>`;
        } else {
          let output = `<table class="table table-dark table-striped mt-3">
                          <thead>
                            <tr>
                              <th>Order ID</th>
                              <th>Order Date</th>
                              <th>Delivery Date</th>
                              <th>Status</th>
                            </tr>
                          </thead>
                          <tbody>`;
          orders.forEach(order => {
            output += `<tr>
                         <td>${order.orderId}</td>
                         <td>${order.orderDate}</td>
                         <td>${order.deliveryDate ? order.deliveryDate : "N/A"}</td>
                         <td>${order.status}</td>
                       </tr>`;
          });
          output += `</tbody></table>`;
          resultDiv.innerHTML = output;
        }
      } catch (error) {
        console.error("Error fetching undelivered orders:", error);
        resultDiv.innerHTML = `<p class="text-danger">Error fetching undelivered orders: ${error.message}</p>`;
      }
    });
  }

  if (getOrdersByUserIdLink) {
    getOrdersByUserIdLink.addEventListener("click", async (event) => {
      event.preventDefault();
      console.log("All Orders link clicked");
      removeWelcomeScreen();
      clearReviewViews();
      const userId = getStoredUserId();
      if (!userId) {
        productList.innerHTML = `<p class="text-warning">User ID is not available. Please log in again.</p>`;
        showElement(productList);
        return;
      }
      productList.innerHTML = `
        <div id="ordersByUserIdContainer" class="container mt-3">
          <h3>Orders for User ID: ${userId}</h3>
          <div id="ordersResult" class="mt-3"></div>
        </div>
      `;
      showElement(productList);
      const resultDiv = document.getElementById("ordersResult");
      const token = localStorage.getItem("jwtToken");
      try {
        const response = await fetch(`https://localhost:7262/api/Order/user/${userId}`, {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
          }
        });
        if (!response.ok) {
          throw new Error(`Error: ${response.status}`);
        }
        const orders = await response.json();
        if (orders.length === 0) {
          resultDiv.innerHTML = `<p class="text-warning">No orders found for your account.</p>`;
        } else {
          let output = `<table class="table table-dark table-striped mt-3">
                          <thead>
                            <tr>
                              <th>Order ID</th>
                              <th>Order Date</th>
                              <th>Delivery Date</th>
                              <th>Status</th>
                            </tr>
                          </thead>
                          <tbody>`;
          orders.forEach(order => {
            output += `<tr>
                         <td>${order.orderId}</td>
                         <td>${order.orderDate}</td>
                         <td>${order.deliveryDate ? order.deliveryDate : "N/A"}</td>
                         <td>${order.status}</td>
                       </tr>`;
          });
          output += `</tbody></table>`;
          resultDiv.innerHTML = output;
        }
      } catch (error) {
        console.error("Error fetching orders:", error);
        resultDiv.innerHTML = `<p class="text-danger">Error fetching orders: ${error.message}</p>`;
      }
    });
  }

// --- Navigation Handlers ---

  // Clicking "Products" loads the products view.
  if (productsLink) {
    productsLink.addEventListener("click", async (event) => {
      event.preventDefault();
      removeWelcomeScreen();
      clearReviewViews();
      hideElement(productList);
      const token = localStorage.getItem("jwtToken");
      try {
        const response = await fetch("https://localhost:7262/api/Product", {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
          }
        });
        if (!response.ok) {
          throw new Error("Unauthorized: You may not have permission to view products.");
        }
        const data = await response.json();
        console.log("Fetched Data:", data);
        if (data.length === 0) {
          productList.innerHTML = `<p class="text-warning text-center mt-3">No products available.</p>`;
          showElement(productList);
          return;
        }
        let output = `<table class="table table-dark table-striped mt-3">
                        <thead>
                          <tr>
                            <th>Product ID</th>
                            <th>Name</th>
                            <th>Price</th>
                            <th>Type</th>
                            <th>Max Quantity</th>
                          </tr>
                        </thead>
                        <tbody>`;
        data.forEach(product => {
          output += `<tr>
                      <td>${product.productId}</td>
                      <td>${product.name}</td>
                      <td>${product.price}</td>
                      <td>${product.type}</td>
                      <td>${product.maxQuantityPerBlock}</td>
                    </tr>`;
        });
        output += `</tbody></table>`;
        productList.innerHTML = output;
        showElement(productList);
      } catch (error) {
        console.error("Error loading products:", error);
        productList.innerHTML = `<p class="text-danger text-center mt-3">${error.message}</p>`;
        showElement(productList);
      }
    });
  }

  // Clicking the Warehouse logo restores the welcome screen and clears review views.
  if (warehouseLogo) {
    warehouseLogo.addEventListener("click", (event) => {
      event.preventDefault();
      productList.innerHTML = "";
      hideElement(productList);
      clearReviewViews();
      restoreWelcomeScreen();
    });
  }

  // Clicking "Write a Review" hides products and initiates the review workflow.
  if (postReviewButton) {
    postReviewButton.addEventListener("click", (event) => {
      event.preventDefault();
      removeWelcomeScreen();
      productList.innerHTML = "";
      hideElement(productList);
      clearReviewViews();
      showElement(ordersContainer);
      showOrdersListForReview();
    });
  }

// -----------------------------------------------------------------------------------------------------//
// --------------------------------------------ADMIN----------------------------------------------------//
// -----------------------------------------------------------------------------------------------------//

  // (If you have other navigation items like "Undelivered Orders" or "All Orders",
  //  add clearReviewViews() calls in their event handlers similarly.)

  // ✅ ADMIN: "Get Stock" functionality
  const getStockLink = document.getElementById("getStock");

  if (getStockLink) {
    getStockLink.addEventListener("click", (event) => {
      event.preventDefault();
      removeWelcomeScreen();
      showElement(productList, "block");

      // Render a neat Get Stock form in productList:
      productList.innerHTML = `
        <div id="getStockForm" class="container mt-3">
          <h3>Get Stock</h3>
          <div class="mb-3">
            <label for="stockProductId" class="form-label">Product ID:</label>
            <input type="number" id="stockProductId" class="form-control" placeholder="Enter Product ID">
          </div>
          <button id="submitStock" class="btn btn-warning">Get Stock</button>
          <div id="stockResult" class="mt-3"></div>
        </div>
      `;
      productList.style.display = "block";

      // Add event listener to the new "Get Stock" form's button.
      const submitStockButton = document.getElementById("submitStock");
      submitStockButton.addEventListener("click", async () => {
        const pid = document.getElementById("stockProductId").value;
        const resultDiv = document.getElementById("stockResult");
        if (!pid) {
          resultDiv.innerHTML = `<p class="text-warning">Please enter a Product ID.</p>`;
          return;
        }
        const token = localStorage.getItem("jwtToken");
        try {
          const response = await fetch(`https://localhost:7262/api/Block/storage/stock/${pid}`, {
            method: "GET",
            headers: {
              "Content-Type": "application/json",
              "Authorization": `Bearer ${token}`
            }
          });
          if (!response.ok) {
            throw new Error(`Error fetching stock. Status: ${response.status}`);
          }
          const stock = await response.json();
          resultDiv.innerHTML = `<p class="text-success">Stock for Product ID ${pid}: ${stock}</p>`;
        } catch (error) {
          console.error("Error fetching stock:", error);
          resultDiv.innerHTML = `<p class="text-danger">Error: ${error.message}</p>`;
        }
      });
    });
  }

  // ✅ ADMIN: "Assign Storage" functionality
  // ADMIN: "Assign Storage" functionality
  // ADMIN: "Assign Storage" functionality
  const assignStorageLink = document.getElementById("assignStorage");
  if (assignStorageLink) {
    assignStorageLink.addEventListener("click", (event) => {
      event.preventDefault();
      removeWelcomeScreen();
      showElement(productList, "block");

      // Render an Assign Storage form into productList without the Max Quantity input:
      productList.innerHTML = `
      <div id="assignStorageForm" class="container mt-3">
        <h3>Assign Storage</h3>
        <div class="mb-3">
          <label for="assignProductId" class="form-label">Product ID:</label>
          <input type="number" id="assignProductId" class="form-control" placeholder="Enter Product ID">
        </div>
        <div class="mb-3">
          <label for="assignQuantity" class="form-label">Quantity to assign:</label>
          <input type="number" id="assignQuantity" class="form-control" placeholder="Enter Quantity">
        </div>
        <button id="submitAssign" class="btn btn-warning">Assign Storage</button>
        <div id="assignResult" class="mt-3"></div>
      </div>
    `;
      showElement(productList, "block");

      // Add event listener to the Assign Storage form's button.
      const submitAssignButton = document.getElementById("submitAssign");
      submitAssignButton.addEventListener("click", async () => {
        const productId = document.getElementById("assignProductId").value;
        const quantityStr = document.getElementById("assignQuantity").value;
        const resultDiv = document.getElementById("assignResult");
        let errorMessage = "";
        if (!productId) errorMessage += "Product ID is required. ";
        if (!quantityStr) errorMessage += "Quantity is required. ";
        if (errorMessage) {
          resultDiv.innerHTML = `<p class="text-warning">${errorMessage}</p>`;
          return;
        }
        const quantity = parseInt(quantityStr, 10);
        if (isNaN(quantity) || quantity < 0) {
          resultDiv.innerHTML = `<p class="text-warning">Quantity must be a non-negative number.</p>`;
          return;
        }
        // Construct the DTO with only productId and quantity.
        const dto = {
          productId: parseInt(productId, 10),
          quantity: quantity
        };

        const token = localStorage.getItem("jwtToken");
        try {
          const response = await fetch("https://localhost:7262/api/Block/storage/assign", {
            method: "PUT",
            headers: {
              "Content-Type": "application/json",
              "Authorization": `Bearer ${token}`
            },
            body: JSON.stringify(dto)
          });
          if (response.ok) {
            resultDiv.innerHTML = `<p class="text-success">Storage assigned successfully!</p>`;
          } else {
            const errorText = await response.text();
            resultDiv.innerHTML = `<p class="text-danger">Error assigning storage: ${errorText}</p>`;
          }
        } catch (error) {
          console.error("Error assigning storage:", error);
          resultDiv.innerHTML = `<p class="text-danger">Error assigning storage: ${error.message}</p>`;
        }
      });
    });
  }

});

// ADMIN: "Remove Storage" functionality
// ADMIN: "Remove Storage" functionality
const removeStorageLink = document.getElementById("removeStorage");
if (removeStorageLink) {
  removeStorageLink.addEventListener("click", (event) => {
    event.preventDefault();
    removeWelcomeScreen();
    showElement(productList, "block");

    // Render a Remove Storage form in productList without the Max Quantity input:
    productList.innerHTML = `
      <div id="removeStorageForm" class="container mt-3">
        <h3>Remove Storage</h3>
        <div class="mb-3">
          <label for="removeProductId" class="form-label">Product ID:</label>
          <input type="number" id="removeProductId" class="form-control" placeholder="Enter Product ID">
        </div>
        <div class="mb-3">
          <label for="removeQuantity" class="form-label">Quantity to remove:</label>
          <input type="number" id="removeQuantity" class="form-control" placeholder="Enter Quantity">
        </div>
        <button id="submitRemove" class="btn btn-warning">Remove Storage</button>
        <div id="removeResult" class="mt-3"></div>
      </div>
    `;
    showElement(productList, "block");

    // Add event listener to the Remove Storage form's button.
    const submitRemoveButton = document.getElementById("submitRemove");
    submitRemoveButton.addEventListener("click", async () => {
      const productId = document.getElementById("removeProductId").value;
      const quantityStr = document.getElementById("removeQuantity").value;
      const resultDiv = document.getElementById("removeResult");

      let errorMessage = "";
      if (!productId) errorMessage += "Product ID is required. ";
      if (!quantityStr) errorMessage += "Quantity is required. ";
      if (errorMessage) {
        resultDiv.innerHTML = `<p class="text-warning">${errorMessage}</p>`;
        return;
      }

      const quantity = parseInt(quantityStr, 10);
      if (isNaN(quantity) || quantity < 0) {
        resultDiv.innerHTML = `<p class="text-warning">Quantity must be a non-negative number.</p>`;
        return;
      }

      // Construct the DTO with only productId and quantity.
      const dto = {
        productId: parseInt(productId, 10),
        quantity: quantity
      };

      const token = localStorage.getItem("jwtToken");
      try {
        const response = await fetch("https://localhost:7262/api/Block/storage/remove", {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
          },
          body: JSON.stringify(dto)
        });
        if (response.ok) {
          resultDiv.innerHTML = `<p class="text-success">Storage removed successfully!</p>`;
        } else {
          const errorText = await response.text();
          resultDiv.innerHTML = `<p class="text-danger">Error removing storage: ${errorText}</p>`;
        }
      } catch (error) {
        console.error("Error removing storage:", error);
        resultDiv.innerHTML = `<p class="text-danger">Error removing storage: ${error.message}</p>`;
      }
    });
  });
}



const getOrderByIdLink = document.getElementById("getOrderById");
if (getOrderByIdLink) {
  getOrderByIdLink.addEventListener("click", (event) => {
    event.preventDefault();
    removeWelcomeScreen();
    showElement(productList, "block");

    productList.innerHTML = `
      <div id="getOrderByIdForm" class="container mt-3">
        <h3>Get Order By ID</h3>
        <div class="mb-3">
          <label for="orderIdInput" class="form-label">Order ID:</label>
          <input type="number" id="orderIdInput" class="form-control" placeholder="Enter Order ID">
        </div>
        <button id="submitOrderById" class="btn btn-warning">Get Order</button>
        <div id="orderResult" class="mt-3"></div>
      </div>
    `;
    productList.style.display = "block";

    document.getElementById("submitOrderById").addEventListener("click", async () => {
      const orderId = document.getElementById("orderIdInput").value;
      const resultDiv = document.getElementById("orderResult");
      if (!orderId) {
        resultDiv.innerHTML = `<p class="text-warning">Please enter a valid Order ID.</p>`;
        return;
      }
      try {
        const token = localStorage.getItem("jwtToken");
        const response = await fetch(`https://localhost:7262/api/Order/${orderId}`, {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
          }
        });

        if (!response.ok) {
          throw new Error(`Order not found. Status: ${response.status}`);
        }
        const order = await response.json();
        resultDiv.innerHTML = `
          <p class="text-success">Order Details:</p>
          <p><strong>Order ID:</strong> ${order.orderId}</p>
          <p><strong>User ID:</strong> ${order.userId}</p>
          <p><strong>Status:</strong> ${order.status}</p>
          <p><strong>Order Date:</strong> ${order.orderDate}</p>
        `;
      } catch (error) {
        console.error("Error fetching order:", error);
        resultDiv.innerHTML = `<p class="text-danger">Error: ${error.message}</p>`;
      }
    });
  });
}

const changeOrderStatusLink = document.getElementById("changeOrderStatus");
if (changeOrderStatusLink) {
  changeOrderStatusLink.addEventListener("click", (event) => {
    event.preventDefault();
    removeWelcomeScreen();
    showElement(productList, "block");

    productList.innerHTML = `
      <div id="changeOrderStatusForm" class="container mt-3">
        <h3>Change Order Status</h3>
        <div class="mb-3">
          <label for="changeOrderId" class="form-label">Order ID:</label>
          <input type="number" id="changeOrderId" class="form-control" placeholder="Enter Order ID">
        </div>
        <div class="mb-3">
          <label for="orderStatusSelect" class="form-label">New Status:</label>
          <select id="orderStatusSelect" class="form-select">
            <option value="0">Pending</option>
            <option value="1">Accepted</option>
            <option value="2">ReadyForDelivery</option>
            <option value="3">Delivered</option>
            <option value="4">Cancelled</option>
            <option value="5">Undeliverable</option>
          </select>
        </div>
        <button id="submitChangeOrderStatus" class="btn btn-warning">Change Status</button>
        <div id="statusChangeResult" class="mt-3"></div>
      </div>
    `;
    productList.style.display = "block";

    document.getElementById("submitChangeOrderStatus").addEventListener("click", async () => {
      const orderId = document.getElementById("changeOrderId").value;
      const newStatus = parseInt(document.getElementById("orderStatusSelect").value, 10);
      const resultDiv = document.getElementById("statusChangeResult");

      if (!orderId) {
        resultDiv.innerHTML = `<p class="text-warning">Order ID is required.</p>`;
        return;
      }

      // Send only the numeric status value
      const dto = { orderStatus: newStatus };

      const token = localStorage.getItem("jwtToken");
      try {
        const response = await fetch(`https://localhost:7262/api/Order/delivery/${orderId}`, {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
          },
          body: JSON.stringify(dto)
        });

        if (response.ok) {
          resultDiv.innerHTML = `<p class="text-success">Order status updated successfully!</p>`;
        } else {
          const errorText = await response.text();
          resultDiv.innerHTML = `<p class="text-danger">Error changing order status: ${errorText}</p>`;
        }
      } catch (error) {
        console.error("Error changing order status:", error);
        resultDiv.innerHTML = `<p class="text-danger">Error: ${error.message}</p>`;
      }
    });
  });
}

const getFeedbackByIdLink = document.getElementById("getFeedbackById");
if (getFeedbackByIdLink) {
  getFeedbackByIdLink.addEventListener("click", (event) => {
    event.preventDefault();
    // Remove or hide the welcome screen if present.
    removeWelcomeScreen();
    showElement(productList, "block");


    // Render a form for entering the Feedback ID.
    productList.innerHTML = `
      <div id="getFeedbackForm" class="container mt-3">
        <h3>Get Feedback By ID</h3>
        <div class="mb-3">
          <label for="feedbackIdInput" class="form-label">Feedback ID:</label>
          <input type="number" id="feedbackIdInput" class="form-control" placeholder="Enter Feedback ID">
        </div>
        <button id="submitFeedbackButton" class="btn btn-warning">Get Feedback</button>
        <div id="feedbackResult" class="mt-3"></div>
      </div>
    `;
    productList.style.display = "block";

    // Add event listener for the submission:
    document.getElementById("submitFeedbackButton").addEventListener("click", async () => {
      const feedbackId = document.getElementById("feedbackIdInput").value;
      const resultDiv = document.getElementById("feedbackResult");
      if (!feedbackId) {
        resultDiv.innerHTML = `<p class="text-warning">Please enter a valid Feedback ID.</p>`;
        return;
      }
      try {
        // Notice the extra closing parenthesis appended here, matching the backend endpoint.
        const response = await fetch(`https://localhost:7262/api/Feedback/reviews/${feedbackId})`, {
          method: "GET",
          headers: {
            "Content-Type": "application/json"
          }
        });
        if (!response.ok) {
          throw new Error(`Feedback not found. Status: ${response.status}`);
        }
        const feedback = await response.json();
        resultDiv.innerHTML = `
          <h4>Feedback Details:</h4>
          <p><strong>ID:</strong> ${feedback.feedbackId}</p>
          <p><strong>Star Rating:</strong> ${feedback.starRating}</p>
          <p><strong>Feedback:</strong> ${feedback.feedbackText ? feedback.feedbackText : "No comment provided."}</p>
        `;
      } catch (error) {
        console.error("Error fetching feedback:", error);
        resultDiv.innerHTML = `<p class="text-danger">Error: ${error.message}</p>`;
      }
    });
  });
}

document.addEventListener("DOMContentLoaded", () => {
  // ---------------------
  // CreateProduct Functionality
  // ---------------------
  const createProductLink = document.getElementById("createProduct");
  if (createProductLink) {
    createProductLink.addEventListener("click", (event) => {
      event.preventDefault();
      removeWelcomeScreen();
      showElement(productList, "block");

      productList.innerHTML = `
        <div id="createProductForm" class="container mt-3">
          <h3>Create Product</h3>
          <div class="mb-3">
            <label for="productName" class="form-label">Name:</label>
            <input type="text" id="productName" class="form-control" placeholder="Enter product name">
          </div>
          <div class="mb-3">
            <label for="productPrice" class="form-label">Price:</label>
            <input type="number" step="0.01" id="productPrice" class="form-control" placeholder="Enter price">
          </div>
          <div class="mb-3">
            <label for="productType" class="form-label">Type:</label>
            <input type="text" id="productType" class="form-control" placeholder="Enter type">
          </div>
          <div class="mb-3">
            <label for="maxQuantity" class="form-label">Max Quantity Per Block:</label>
            <input type="number" id="maxQuantity" class="form-control" placeholder="Enter max quantity per block">
          </div>
          <button id="submitCreateProduct" class="btn btn-warning">Create Product</button>
          <div id="createProductResult" class="mt-3"></div>
        </div>
      `;
      productList.style.display = "block";

      document.getElementById("submitCreateProduct").addEventListener("click", async () => {
        const name = document.getElementById("productName").value;
        const price = document.getElementById("productPrice").value;
        const type = document.getElementById("productType").value;
        const maxQuantity = document.getElementById("maxQuantity").value;
        const resultDiv = document.getElementById("createProductResult");

        // Basic validation: ensure all fields are filled in.
        if (!name || !price || !type || !maxQuantity) {
          resultDiv.innerHTML = `<p class="text-warning">All fields are required.</p>`;
          return;
        }

        // Construct the DTO according to ProductCreateDTO
        const dto = {
          Name: name,
          Price: parseFloat(price),
          Type: type,
          MaxQuantityPerBlock: parseInt(maxQuantity, 10)
        };

        // Retrieve the JWT token stored locally (should be an Admin token)
        const token = localStorage.getItem("jwtToken");
        try {
          const response = await fetch("https://localhost:7262/api/Product", {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
              "Authorization": `Bearer ${token}`
            },
            body: JSON.stringify(dto)
          });

          if (!response.ok) {
            const errorText = await response.text();
            resultDiv.innerHTML = `<p class="text-danger">Error creating product: ${errorText}</p>`;
          } else {
            const product = await response.json();
            resultDiv.innerHTML = `
              <h4>Product Created Successfully!</h4>
              <p><strong>ID:</strong> ${product.productId}</p>
              <p><strong>Name:</strong> ${product.name}</p>
              <p><strong>Price:</strong> ${product.price}</p>
              <p><strong>Type:</strong> ${product.type}</p>
              <p><strong>Max Quantity Per Block:</strong> ${product.maxQuantityPerBlock}</p>
            `;
          }
        } catch (error) {
          console.error("Error creating product:", error);
          resultDiv.innerHTML = `<p class="text-danger">Error: ${error.message}</p>`;
        }
      });
    });
  }

  // ---------------------
  // GetProductById Functionality
  // ---------------------
  const getProductByIdLink = document.getElementById("getProductById");
  if (getProductByIdLink) {
    getProductByIdLink.addEventListener("click", (event) => {
      event.preventDefault();
      removeWelcomeScreen();
      showElement(productList, "block");

      productList.innerHTML = `
        <div id="getProductForm" class="container mt-3">
          <h3>Get Product By ID</h3>
          <div class="mb-3">
            <label for="productIdInput" class="form-label">Product ID:</label>
            <input type="number" id="productIdInput" class="form-control" placeholder="Enter Product ID">
          </div>
          <button id="submitGetProduct" class="btn btn-warning">Get Product</button>
          <div id="getProductResult" class="mt-3"></div>
        </div>
      `;
      productList.style.display = "block";

      document.getElementById("submitGetProduct").addEventListener("click", async () => {
        const productId = document.getElementById("productIdInput").value;
        const resultDiv = document.getElementById("getProductResult");
        if (!productId) {
          resultDiv.innerHTML = `<p class="text-warning">Please enter a valid Product ID.</p>`;
          return;
        }
        try {
          const response = await fetch(`https://localhost:7262/api/Product/${productId}`, {
            method: "GET",
            headers: {
              "Content-Type": "application/json"
            }
          });
          if (!response.ok) {
            throw new Error(`Product not found. Status: ${response.status}`);
          }
          const product = await response.json();
          resultDiv.innerHTML = `
            <h4>Product Details:</h4>
            <p><strong>ID:</strong> ${product.productId}</p>
            <p><strong>Name:</strong> ${product.name}</p>
            <p><strong>Price:</strong> ${product.price}</p>
            <p><strong>Type:</strong> ${product.type}</p>
            <p><strong>Max Quantity Per Block:</strong> ${product.maxQuantityPerBlock}</p>
          `;
        } catch (error) {
          console.error("Error fetching product:", error);
          resultDiv.innerHTML = `<p class="text-danger">Error: ${error.message}</p>`;
        }
      });
    });
  }
});
// -----------------------------------------------------------------------------------------------------//
// ------------------------------------------CUSTOMER---------------------------------------------------//
// -----------------------------------------------------------------------------------------------------//
document.addEventListener("DOMContentLoaded", () => {
  // Cache elements from your HTML.
  const getUndeliveredOrdersByUserIdLink = document.getElementById("getUndeliveredOrdersByUserId");
  const getOrdersByUserIdLink = document.getElementById("getOrdersByUserId");
  const productList = document.getElementById("productList");

  // ---------------------
  // Get Undelivered Orders By User ID
  // ---------------------
  if (getUndeliveredOrdersByUserIdLink) {
    getUndeliveredOrdersByUserIdLink.addEventListener("click", async (event) => {
      event.preventDefault();
      console.log("Undelivered Orders link clicked");

      // Make sure these helper functions exist and are defined in your code.
      removeWelcomeScreen();
      clearReviewViews();

      const userId = getStoredUserId();
      console.log("User ID for undelivered orders:", userId);
      if (!userId) {
        productList.innerHTML = `<p class="text-warning">User ID is not available. Please log in again.</p>`;
        productList.style.display = "block";
        return;
      }

      productList.innerHTML = `
        <div id="undeliveredOrdersByUserIdContainer" class="container mt-3">
          <h3>Undelivered Orders for User ID: ${userId}</h3>
          <div id="undeliveredOrdersResult" class="mt-3"></div>
        </div>
      `;
      productList.style.display = "block";

      const resultDiv = document.getElementById("undeliveredOrdersResult");
      console.log("Undelivered orders result div:", resultDiv);
      const token = localStorage.getItem("jwtToken");

      try {
        const response = await fetch(`https://localhost:7262/api/Order/delivery/user/${userId}`, {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
          }
        });
        console.log("Undelivered Orders Response:", response);
        if (!response.ok) {
          throw new Error(`Error: ${response.status}`);
        }
        const orders = await response.json();
        console.log("Undelivered Orders:", orders);
        if (orders.length === 0) {
          resultDiv.innerHTML = `<p class="text-warning">No undelivered orders found for your account.</p>`;
        } else {
          let output = `<table class="table table-dark table-striped mt-3">
                          <thead>
                            <tr>
                              <th>Order ID</th>
                              <th>Order Date</th>
                              <th>Delivery Date</th>
                              <th>Status</th>
                            </tr>
                          </thead>
                          <tbody>`;
          orders.forEach(order => {
            output += `<tr>
                         <td>${order.orderId}</td>
                         <td>${order.orderDate}</td>
                         <td>${order.deliveryDate ? order.deliveryDate : "N/A"}</td>
                         <td>${order.status}</td>
                       </tr>`;
          });
          output += `</tbody></table>`;
          resultDiv.innerHTML = output;
          console.log("Filled undeliveredOrdersResult with table.");
        }
      } catch (error) {
        console.error("Error fetching undelivered orders:", error);
        resultDiv.innerHTML = `<p class="text-danger">Error fetching undelivered orders: ${error.message}</p>`;
      }
    });
  }

  // ---------------------
  // Get All Orders By User ID
  // ---------------------
  if (getOrdersByUserIdLink) {
    getOrdersByUserIdLink.addEventListener("click", async (event) => {
      event.preventDefault();
      console.log("All Orders link clicked");
      removeWelcomeScreen();
      clearReviewViews();

      const userId = getStoredUserId();
      console.log("User ID for all orders:", userId);
      if (!userId) {
        productList.innerHTML = `<p class="text-warning">User ID is not available. Please log in again.</p>`;
        productList.style.display = "block";
        return;
      }

      productList.innerHTML = `
        <div id="ordersByUserIdContainer" class="container mt-3">
          <h3>Orders for User ID: ${userId}</h3>
          <div id="ordersResult" class="mt-3"></div>
        </div>
      `;
      productList.style.display = "block";

      // Adding a debug log to verify that the container is in the DOM.
      const resultDiv = document.getElementById("ordersResult");
      if (!resultDiv) {
        console.error("ordersResult container not found!");
        return;
      } else {
        console.log("ordersResult container found:", resultDiv);
      }

      const token = localStorage.getItem("jwtToken");

      try {
        const response = await fetch(`https://localhost:7262/api/Order/user/${userId}`, {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
          }
        });
        console.log("All Orders Response:", response);
        if (!response.ok) {
          throw new Error(`Error: ${response.status}`);
        }
        const orders = await response.json();
        console.log("All Orders:", orders);
        if (orders.length === 0) {
          resultDiv.innerHTML = `<p class="text-warning">No orders found for your account.</p>`;
        } else {
          let output = `<table class="table table-dark table-striped mt-3">
                          <thead>
                            <tr>
                              <th>Order ID</th>
                              <th>Order Date</th>
                              <th>Delivery Date</th>
                              <th>Status</th>
                            </tr>
                          </thead>
                          <tbody>`;
          orders.forEach(order => {
            output += `<tr>
                         <td>${order.orderId}</td>
                         <td>${order.orderDate}</td>
                         <td>${order.deliveryDate ? order.deliveryDate : "N/A"}</td>
                         <td>${order.status}</td>
                       </tr>`;
          });
          output += `</tbody></table>`;
          resultDiv.innerHTML = output;
          console.log("Filled ordersResult with table.");
        }
      } catch (error) {
        console.error("Error fetching orders:", error);
        resultDiv.innerHTML = `<p class="text-danger">Error fetching orders: ${error.message}</p>`;
      }
    });
  }
});




async function showOrdersListForReview() {
  const userId = getStoredUserId();
  const token = localStorage.getItem("jwtToken");
  const ordersContainer = document.getElementById("ordersContainer");

  ordersContainer.innerHTML = "<p>Loading your orders...</p>";

  try {
    const response = await fetch(`https://localhost:7262/api/Order/user/${userId}`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
      }
    });
    if (!response.ok) {
      throw new Error(`Unable to fetch orders: ${response.status}`);
    }
    const orders = await response.json();

    if (orders.length === 0) {
      ordersContainer.innerHTML = `<p>No orders found for your account.</p>`;
      return;
    }

    let html = `<table class="table table-dark table-striped mt-3">
                  <thead>
                    <tr>
                      <th>Order ID</th>
                      <th>Order Date</th>
                      <th>Status</th>
                    </tr>
                  </thead>
                  <tbody>`;
    orders.forEach(order => {
      html += `<tr data-orderid="${order.orderId}" class="order-row" style="cursor:pointer;">
                 <td>${order.orderId}</td>
                 <td>${order.orderDate}</td>
                 <td>${order.status}</td>
               </tr>`;
    });
    html += `</tbody></table>`;
    ordersContainer.innerHTML = html;

  } catch (error) {
    ordersContainer.innerHTML = `<p class="text-danger">Error: ${error.message}</p>`;
  }
}

// Use event delegation for clicks on order rows.
const ordersContainer = document.getElementById("ordersContainer");
ordersContainer.addEventListener("click", (event) => {
  let targetElement = event.target;
  while (targetElement && targetElement !== ordersContainer) {
    if (targetElement.classList && targetElement.classList.contains("order-row")) {
      const orderId = targetElement.getAttribute("data-orderid");
      console.log("Delegated event: Order row clicked:", orderId);
      showReviewForm(orderId);
      break;
    }
    targetElement = targetElement.parentNode;
  }
});

// Define this function inside your DOMContentLoaded callback (or within showReviewForm)
async function submitReview(event) {
  event.preventDefault();
  const orderId = getStoredUserId();
  const starRating = document.getElementById("starRating").value;
  const feedbackText = document.getElementById("feedbackText").value;
  const reviewData = {
    starRating: Number(starRating),
    feedbackText: feedbackText,
    orderId: orderId  // Allows backend to associate the review with the order.
  };

  try {
    const token = localStorage.getItem("jwtToken");
    const response = await fetch("https://localhost:7262/api/Feedback/reviews", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
      },
      body: JSON.stringify(reviewData)
    });
    if (!response.ok) {
      throw new Error(`Failed to submit review: ${response.status}`);
    }
    const result = await response.json();
    console.log("Review added successfully:", result);
    reviewFormContainer.innerHTML = `<p class="text-success">Your review has been submitted successfully!</p>`;
  } catch (error) {
    console.error("Failed to submit review:", error);
    reviewFormContainer.innerHTML = `<p class="text-danger">Failed to submit review: ${error.message}</p>`;
  }
}





const getUndeliveredOrdersLink = document.getElementById("getUndeliveredOrders");
if (getUndeliveredOrdersLink) {
  getUndeliveredOrdersLink.addEventListener("click", async (event) => {
    event.preventDefault();
    removeWelcomeScreen();
    showElement(productList, "block");
    const token = localStorage.getItem("jwtToken");
    try {
      const response = await fetch("https://localhost:7262/api/Order/delivery", {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${token}`
        }
      });

      if (!response.ok) {
        throw new Error("Error fetching undelivered orders.");
      }
      const orders = await response.json();

      if (orders.length === 0) {
        productList.innerHTML = `<p class="text-warning">No undelivered orders found.</p>`;
      } else {
        let output = `<table class="table table-dark table-striped mt-3">
                        <thead>
                          <tr>
                            <th>Order ID</th>
                            <th>User ID</th>
                            <th>Status</th>
                            <th>Order Date</th>
                          </tr>
                        </thead>
                        <tbody>`;
        orders.forEach(order => {
          output += `<tr>
                      <td>${order.orderId}</td>
                      <td>${order.userId}</td>
                      <td>${order.status}</td>
                      <td>${order.orderDate}</td>
                    </tr>`;
        });
        output += `</tbody></table>`;
        productList.innerHTML = output;
      }
      productList.style.display = "block";
    } catch (error) {
      console.error("Error fetching undelivered orders:", error);
      productList.innerHTML = `<p class="text-danger">Error fetching orders: ${error.message}</p>`;
    }
  });
}


// Utility: Remove the current welcome screen.
function removeWelcomeScreen() {
  const currentWelcome = document.getElementById("welcomeScreen");
  if (currentWelcome) {
    currentWelcome.remove();
  }
}





// ✅ Function to update the user link dynamically based on login status.
function updateUserLink() {
  const userLink = document.getElementById("userLink");
  if (!userLink) return;
  const token = localStorage.getItem("jwtToken");
  const expireTime = localStorage.getItem("jwtExpireTime");
  const newUserLink = userLink.cloneNode(true);
  userLink.parentNode.replaceChild(newUserLink, userLink);
  if (token && Date.now() < expireTime) {
    newUserLink.textContent = "Logout";
    newUserLink.href = "#";
    newUserLink.addEventListener("click", (event) => {
      event.preventDefault();
      localStorage.removeItem("jwtToken");
      localStorage.removeItem("jwtExpireTime");
      localStorage.removeItem("userId");
      alert("Logged out successfully!");
      updateUserLink();
      updateRoleMenus();
    });
  } else {
    newUserLink.textContent = "Login";
    newUserLink.addEventListener("click", (event) => {
      event.preventDefault();
      window.location.href = "/src/User/login.html";
    });
  }
  updateRoleMenus();
}

// ✅ Function to update header dropdown menus based on roles in the JWT.
function updateRoleMenus() {
  const token = localStorage.getItem("jwtToken");
  const customerMenuLi = document.getElementById("customerMenu");
  const adminMenuLi = document.getElementById("adminMenu");
  const transporterMenuLi = document.getElementById("transporterMenu");
  const supplierMenuLi = document.getElementById("supplierMenu");
  if (customerMenuLi) customerMenuLi.style.display = "none";
  if (adminMenuLi) adminMenuLi.style.display = "none";
  if (transporterMenuLi) transporterMenuLi.style.display = "none";
  if (supplierMenuLi) supplierMenuLi.style.display = "none";
  if (!token) return;
  let roles = [];
  try {
    const decoded = parseJwt(token);
    roles = decoded.role || decoded.roles || decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || [];
    if (typeof roles === "string") roles = [roles];
  } catch (e) {
    console.error("Error decoding token:", e);
    return;
  }
  roles = roles.map(r => r.toLowerCase());
  console.log("Decoded roles:", roles);
  if (roles.includes("customer") && customerMenuLi) {
    customerMenuLi.style.display = "block";
  }
  if (roles.includes("admin") && adminMenuLi) {
    adminMenuLi.style.display = "block";
  }
  if (roles.includes("transporter") && transporterMenuLi) {
    transporterMenuLi.style.display = "block";
  }
  if (roles.includes("supplier") && supplierMenuLi) {
    supplierMenuLi.style.display = "block";
  }
}






