const apiUrl = "https://localhost:7203/api/products";

document.addEventListener("DOMContentLoaded", function () {
    fetchProducts();

    document.getElementById("btnAdd")
        .addEventListener("click", addProduct);
});

// Lấy danh sách sản phẩm
function fetchProducts() {
    fetch(apiUrl)
        .then(handleResponse)
        .then(data => displayProducts(data))
        .catch(error => console.error("Fetch error:", error));
}

// Xử lý response
function handleResponse(response) {
    if (!response.ok) {
        throw new Error("Network response was not ok");
    }
    return response.json();
}

// Hiển thị danh sách sản phẩm
function displayProducts(products) {
    const productList = document.getElementById("productList");

    productList.innerHTML = "";

    products.forEach(product => {
        productList.innerHTML += createProductRow(product);
    });
}

// Tạo dòng dữ liệu
function createProductRow(product) {
    return `
        <tr>
            <td>${product.id}</td>
            <td>${product.name}</td>
            <td>${product.price}</td>
            <td>${product.description}</td>
            <td>${product.imageUrl ?? ""}</td>
            <td>${product.categoryId}</td>
            <td>
                <button class="btn btn-danger"
                        onclick="deleteProduct(${product.id})">
                    Xóa
                </button>
            </td>
        </tr>
    `;
}

// Thêm sản phẩm
function addProduct() {

    const productData = {
        name: document.getElementById("name").value,
        price: parseFloat(document.getElementById("price").value),
        description: document.getElementById("description").value,
        imageUrl: document.getElementById("imageUrl").value,
        categoryId: parseInt(document.getElementById("categoryId").value)
    };

    fetch(apiUrl, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(productData)
    })
    .then(handleResponse)
    .then(data => {
        console.log("Product added:", data);

        document.getElementById("name").value = "";
        document.getElementById("price").value = "";
        document.getElementById("description").value = "";
        document.getElementById("imageUrl").value = "";
        document.getElementById("categoryId").value = "";

        fetchProducts();
    })
    .catch(error => console.error("Error:", error));
}

// Xóa sản phẩm
function deleteProduct(id) {

    if (!confirm("Bạn có chắc muốn xóa sản phẩm này?")) {
        return;
    }

    fetch(`${apiUrl}/${id}`, {
        method: "DELETE"
    })
    .then(handleResponse)
    .then(() => {
        fetchProducts();
    })
    .catch(error => console.error("Delete error:", error));
}