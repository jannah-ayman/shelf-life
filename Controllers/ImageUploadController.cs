using Microsoft.AspNetCore.Mvc;

namespace ShelfLife.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const int MaxBookPhotos = 5;

        public ImageUploadController(IWebHostEnvironment env)
        {
            _env = env;
        }

        // POST: api/ImageUpload/profile
        [HttpPost("profile")]
        public async Task<ActionResult<ImageUploadResponseDTO>> UploadProfilePicture(IFormFile file)
        {
            var validation = ValidateImage(file);
            if (validation != null)
                return validation;

            try
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "profiles");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName).ToLower()}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var imageUrl = $"/uploads/profiles/{fileName}";
                return Ok(new ImageUploadResponseDTO
                {
                    Success = true,
                    ImageUrl = imageUrl,
                    Message = "Profile picture uploaded successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ImageUploadResponseDTO
                {
                    Success = false,
                    Message = $"Upload failed: {ex.Message}"
                });
            }
        }

        // POST: api/ImageUpload/book
        [HttpPost("book")]
        public async Task<ActionResult<ImageUploadResponseDTO>> UploadBookPhoto(IFormFile file)
        {
            var validation = ValidateImage(file);
            if (validation != null)
                return validation;

            try
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "books");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName).ToLower()}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var imageUrl = $"/uploads/books/{fileName}";

                return Ok(new ImageUploadResponseDTO
                {
                    Success = true,
                    ImageUrl = imageUrl,
                    Message = "Book photo uploaded successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ImageUploadResponseDTO
                {
                    Success = false,
                    Message = $"Upload failed: {ex.Message}"
                });
            }
        }

        // POST: api/ImageUpload/book-multiple
        [HttpPost("book-multiple")]
        public async Task<ActionResult<MultipleImageUploadResponseDTO>> UploadMultipleBookPhotos(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return BadRequest(new MultipleImageUploadResponseDTO
                {
                    Success = false,
                    Message = "No files provided"
                });

            if (files.Count > MaxBookPhotos)
                return BadRequest(new MultipleImageUploadResponseDTO
                {
                    Success = false,
                    Message = $"Maximum {MaxBookPhotos} photos allowed"
                });

            var uploadedImages = new List<ImageUrlPair>();
            var errors = new List<string>();

            foreach (var file in files)
            {
                var validation = ValidateImage(file);
                if (validation != null)
                {
                    errors.Add($"{file.FileName}: Invalid file");
                    continue;
                }

                try
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "books");
                    Directory.CreateDirectory(uploadsFolder);

                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName).ToLower()}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    uploadedImages.Add(new ImageUrlPair
                    {
                        ImageUrl = $"/uploads/books/{fileName}"
                    });
                }
                catch (Exception ex)
                {
                    errors.Add($"{file.FileName}: {ex.Message}");
                }
            }

            return Ok(new MultipleImageUploadResponseDTO
            {
                Success = uploadedImages.Count > 0,
                Images = uploadedImages,
                Message = uploadedImages.Count > 0
                    ? $"Successfully uploaded {uploadedImages.Count} of {files.Count} images"
                    : "All uploads failed",
                Errors = errors.Count > 0 ? errors : null
            });
        }

        // DELETE: api/ImageUpload
        [HttpDelete]
        public ActionResult DeleteImage([FromQuery] string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return BadRequest(new { message = "Image URL is required" });

            try
            {
                // Remove leading slash if present
                var relativePath = imageUrl.TrimStart('/');
                var filePath = Path.Combine(_env.WebRootPath, relativePath);

                if (!System.IO.File.Exists(filePath))
                    return NotFound(new { message = "Image not found" });

                // Delete image
                System.IO.File.Delete(filePath);

                return Ok(new { message = "Image deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Delete failed: {ex.Message}" });
            }
        }

        private ActionResult? ValidateImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file provided" });

            if (file.Length > MaxFileSize)
                return BadRequest(new { message = $"File size exceeds {MaxFileSize / 1024 / 1024}MB limit" });

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!AllowedExtensions.Contains(extension))
                return BadRequest(new { message = $"Invalid file type. Allowed: {string.Join(", ", AllowedExtensions)}" });

            return null;
        }
    }

    // DTOs
    public class ImageUploadResponseDTO
    {
        public bool Success { get; set; }
        public string? ImageUrl { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class MultipleImageUploadResponseDTO
    {
        public bool Success { get; set; }
        public List<ImageUrlPair> Images { get; set; } = new();
        public string Message { get; set; } = string.Empty;
        public List<string>? Errors { get; set; }
    }

    public class ImageUrlPair
    {
        public string ImageUrl { get; set; } = string.Empty;
    }
}