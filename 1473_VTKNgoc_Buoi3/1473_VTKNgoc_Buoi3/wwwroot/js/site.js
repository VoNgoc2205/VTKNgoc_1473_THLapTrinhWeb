document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll('input[type="file"][name="imageUrl"]').forEach((input) => {
        input.addEventListener("change", () => {
            const file = input.files && input.files[0];

            if (!file || !file.type.startsWith("image/")) {
                return;
            }

            const imageUrl = URL.createObjectURL(file);
            const dropBox = input.closest(".upload-drop");
            const editImages = input.closest(".edit-images");

            if (dropBox) {
                dropBox.classList.add("has-preview");
                dropBox.querySelector(".upload-preview")?.remove();

                const image = document.createElement("img");
                image.className = "upload-preview";
                image.src = imageUrl;
                image.alt = "Anh da chon";
                image.onload = () => URL.revokeObjectURL(imageUrl);

                dropBox.appendChild(image);
                return;
            }

            if (editImages) {
                const previewBox = editImages.querySelector(".edit-image-main");
                if (!previewBox) {
                    return;
                }

                previewBox.querySelector("img")?.remove();

                const image = document.createElement("img");
                image.src = imageUrl;
                image.alt = "Anh da chon";
                image.onload = () => URL.revokeObjectURL(imageUrl);

                previewBox.appendChild(image);
            }
        });
    });
});
