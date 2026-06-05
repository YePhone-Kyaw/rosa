using Amazon.S3;
using Amazon.S3.Model;

namespace backend.Services;

public class S3Service
{
    private readonly IAmazonS3 _s3;
    private readonly string _bucketName;

    public S3Service(IAmazonS3 s3, IConfiguration config)
    {
        _s3 = s3;
        _bucketName = config["AWS:BucketName"]!;
    }

    public async Task<string> UploadFileAsync(IFormFile file, string folder)
    {
        var key = $"{folder}/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        var stream = file.OpenReadStream();
        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = stream,
            ContentType = file.ContentType
        };

        await _s3.PutObjectAsync(request);

        return $"https://{_bucketName}.s3.amazonaws.com/{key}";
    }

    public async Task DeleteFileAsync(string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl)) return;
        var uri = new Uri(fileUrl);
        var key = uri.AbsolutePath.TrimStart('/');

        await _s3.DeleteObjectAsync(_bucketName, key);
    }
}
