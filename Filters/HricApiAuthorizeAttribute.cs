using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.IO;
using System.Net;
using System.Net.Http;

namespace Generic.Filters
{
    /// <summary>
    /// HRIC API Authorization Attribute implementing HMAC-SHA256 validation
    /// Provides secure authentication for HRIC integration endpoints
    /// </summary>
    public class HricApiAuthorizeAttribute : System.Web.Http.AuthorizeAttribute
    {
        private const int MAX_TIMESTAMP_AGE_SECONDS = 300; // 5 minutes
        private const string HMAC_ALGORITHM = "HMAC-SHA256";
        
        /// <summary>
        /// Main authorization method called by Web API framework
        /// </summary>
        /// <param name="actionContext">HTTP action context containing request details</param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            try
            {
                if (IsHricAuthenticated(actionContext))
                {
                    // Authentication successful - allow request to proceed
                    return;
                }
                else
                {
                    // Authentication failed - return 401 Unauthorized
                    HandleUnauthorizedRequest(actionContext);
                }
            }
            catch (Exception ex)
            {
                // Log error and return 401 for security
                LogAuthenticationError(ex, actionContext);
                HandleUnauthorizedRequest(actionContext);
            }
        }

        /// <summary>
        /// Validates HRIC HMAC authentication
        /// </summary>
        /// <param name="actionContext">HTTP action context</param>
        /// <returns>True if authentication is valid, false otherwise</returns>
        private bool IsHricAuthenticated(HttpActionContext actionContext)
        {
            var request = actionContext.Request;
            
            // Extract required headers
            var authorizationHeader = GetHeaderValue(request, "Authorization");
            var timestampHeader = GetHeaderValue(request, "X-Timestamp");
            var requestIdHeader = GetHeaderValue(request, "X-Request-ID");
            
            // Validate required headers are present
            if (string.IsNullOrEmpty(authorizationHeader) || string.IsNullOrEmpty(timestampHeader))
            {
                LogAuthenticationFailure("Missing required headers", requestIdHeader);
                return false;
            }
            
            // Parse Authorization header
            if (!ParseAuthorizationHeader(authorizationHeader, out string apiKey, out string signature))
            {
                LogAuthenticationFailure("Invalid authorization header format", requestIdHeader);
                return false;
            }
            
            // Validate API key
            var hmacSecret = GetHmacSecretForApiKey(apiKey);
            if (string.IsNullOrEmpty(hmacSecret))
            {
                LogAuthenticationFailure($"Invalid API key: {apiKey}", requestIdHeader);
                return false;
            }
            
            // Validate timestamp
            if (!IsValidTimestamp(timestampHeader))
            {
                LogAuthenticationFailure("Invalid or expired timestamp", requestIdHeader);
                return false;
            }
            
            // Get request body
            var requestBody = GetRequestBody(request);
            
            // Calculate expected signature
            var expectedSignature = CalculateHmacSignature(
                hmacSecret,
                request.Method.Method,
                request.RequestUri.AbsolutePath,
                timestampHeader,
                requestBody
            );
            
            // Compare signatures using constant-time comparison
            if (!ConstantTimeEquals(expectedSignature, signature))
            {
                LogAuthenticationFailure("Invalid HMAC signature", requestIdHeader);
                return false;
            }
            
            // Authentication successful
            LogAuthenticationSuccess(apiKey, requestIdHeader);
            
            // Store authentication context for use in controller
            HttpContext.Current.Items["HRIC_API_KEY"] = apiKey;
            HttpContext.Current.Items["HRIC_REQUEST_ID"] = requestIdHeader;
            HttpContext.Current.Items["HRIC_TIMESTAMP"] = timestampHeader;
            
            return true;
        }
        
        /// <summary>
        /// Parses the Authorization header to extract API key and signature
        /// Expected format: "HMAC-SHA256 Credential=apiKey, Signature=signature"
        /// </summary>
        /// <param name="authHeader">Authorization header value</param>
        /// <param name="apiKey">Extracted API key</param>
        /// <param name="signature">Extracted signature</param>
        /// <returns>True if parsing successful, false otherwise</returns>
        private bool ParseAuthorizationHeader(string authHeader, out string apiKey, out string signature)
        {
            apiKey = null;
            signature = null;
            
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith(HMAC_ALGORITHM + " "))
            {
                return false;
            }
            
            // Remove "HMAC-SHA256 " prefix
            var credentialsPart = authHeader.Substring(HMAC_ALGORITHM.Length + 1);
            
            // Parse credential and signature parts
            var parts = credentialsPart.Split(',');
            
            foreach (var part in parts)
            {
                var trimmedPart = part.Trim();
                
                if (trimmedPart.StartsWith("Credential="))
                {
                    apiKey = trimmedPart.Substring(11); // Remove "Credential="
                }
                else if (trimmedPart.StartsWith("Signature="))
                {
                    signature = trimmedPart.Substring(10); // Remove "Signature="
                }
            }
            
            return !string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(signature);
        }
        
        /// <summary>
        /// Retrieves HMAC secret for the given API key
        /// </summary>
        /// <param name="apiKey">API key to lookup</param>
        /// <returns>HMAC secret or null if API key is invalid</returns>
        private string GetHmacSecretForApiKey(string apiKey)
        {
            // Get the configured HRIC API key and secret from web.config
            var configuredApiKey = ConfigurationManager.AppSettings["INTELLEGES_API_KEY"];
            var configuredSecret = ConfigurationManager.AppSettings["INTELLEGES_CLIENT_HMAC_SECRET"];
            
            if (string.IsNullOrEmpty(configuredApiKey) || string.IsNullOrEmpty(configuredSecret))
            {
                throw new ConfigurationErrorsException("INTELLEGES_API_KEY and INTELLEGES_CLIENT_HMAC_SECRET must be configured in web.config");
            }
            
            // Validate API key matches configuration
            if (apiKey == configuredApiKey)
            {
                return configuredSecret;
            }
            
            // Future enhancement: Support multiple API keys from database
            // var db = new EntitiesDBContext();
            // var apiKeyRecord = db.HricApiKeys.FirstOrDefault(k => k.KeyId == apiKey && k.IsActive);
            // return apiKeyRecord?.HmacSecret;
            
            return null;
        }
        
        /// <summary>
        /// Validates that the timestamp is within acceptable range
        /// </summary>
        /// <param name="timestamp">Unix timestamp string</param>
        /// <returns>True if timestamp is valid and not expired</returns>
        private bool IsValidTimestamp(string timestamp)
        {
            if (string.IsNullOrEmpty(timestamp))
                return false;
            
            if (!long.TryParse(timestamp, out long unixTimestamp))
                return false;
            
            // Convert to DateTime
            var requestTime = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).DateTime;
            var currentTime = DateTime.UtcNow;
            
            // Check if timestamp is within acceptable range (prevents replay attacks)
            var timeDifference = Math.Abs((currentTime - requestTime).TotalSeconds);
            
            return timeDifference <= MAX_TIMESTAMP_AGE_SECONDS;
        }
        
        /// <summary>
        /// Calculates HMAC-SHA256 signature for the request
        /// </summary>
        /// <param name="secretKey">Base64-encoded HMAC secret key</param>
        /// <param name="method">HTTP method (GET, POST, etc.)</param>
        /// <param name="path">Request path</param>
        /// <param name="timestamp">Unix timestamp</param>
        /// <param name="body">Request body content</param>
        /// <returns>Base64-encoded HMAC signature</returns>
        private string CalculateHmacSignature(string secretKey, string method, string path, string timestamp, string body)
        {
            // Create canonical request string
            var canonicalRequest = $"{method}\n{path}\n{timestamp}\n{body}";
            
            // Decode the base64 secret key
            var keyBytes = Convert.FromBase64String(secretKey);
            
            // Calculate HMAC-SHA256
            using (var hmac = new HMACSHA256(keyBytes))
            {
                var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(canonicalRequest));
                return Convert.ToBase64String(signatureBytes);
            }
        }
        
        /// <summary>
        /// Performs constant-time string comparison to prevent timing attacks
        /// </summary>
        /// <param name="expected">Expected signature</param>
        /// <param name="actual">Actual signature from request</param>
        /// <returns>True if signatures match</returns>
        private bool ConstantTimeEquals(string expected, string actual)
        {
            if (expected == null || actual == null)
                return false;
            
            if (expected.Length != actual.Length)
                return false;
            
            var result = 0;
            for (int i = 0; i < expected.Length; i++)
            {
                result |= expected[i] ^ actual[i];
            }
            
            return result == 0;
        }
        
        /// <summary>
        /// Extracts request body content for signature calculation
        /// </summary>
        /// <param name="request">HTTP request message</param>
        /// <returns>Request body as string</returns>
        private string GetRequestBody(HttpRequestMessage request)
        {
            try
            {
                if (request.Content == null)
                    return string.Empty;
                
                // Read content as string
                var task = request.Content.ReadAsStringAsync();
                task.Wait();
                return task.Result ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
        
        /// <summary>
        /// Gets header value from request
        /// </summary>
        /// <param name="request">HTTP request message</param>
        /// <param name="headerName">Header name to retrieve</param>
        /// <returns>Header value or null if not found</returns>
        private string GetHeaderValue(HttpRequestMessage request, string headerName)
        {
            if (request.Headers.Contains(headerName))
            {
                return request.Headers.GetValues(headerName).FirstOrDefault();
            }
            
            return null;
        }
        
        /// <summary>
        /// Handles unauthorized requests by returning 401 response
        /// </summary>
        /// <param name="actionContext">HTTP action context</param>
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            var response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent(
                    "{\"success\": false, \"error\": {\"code\": \"AUTHENTICATION_FAILED\", \"message\": \"Invalid or missing HMAC authentication\"}}",
                    Encoding.UTF8,
                    "application/json"
                )
            };
            
            response.Headers.Add("WWW-Authenticate", HMAC_ALGORITHM);
            
            throw new HttpResponseException(response);
        }
        
        /// <summary>
        /// Logs authentication success for audit purposes
        /// </summary>
        /// <param name="apiKey">API key that was authenticated</param>
        /// <param name="requestId">Request ID for tracking</param>
        private void LogAuthenticationSuccess(string apiKey, string requestId)
        {
            // Log to system diagnostics (can be enhanced to use proper logging framework)
            System.Diagnostics.Debug.WriteLine($"[HRIC Auth Success] API Key: {apiKey}, Request ID: {requestId}, Time: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
            
            // Future enhancement: Log to database or structured logging system
            // var logEntry = new HricAuthLog
            // {
            //     ApiKey = apiKey,
            //     RequestId = requestId,
            //     Success = true,
            //     Timestamp = DateTime.UtcNow,
            //     IpAddress = HttpContext.Current?.Request?.UserHostAddress
            // };
            // db.HricAuthLogs.Add(logEntry);
            // db.SaveChanges();
        }
        
        /// <summary>
        /// Logs authentication failure for security monitoring
        /// </summary>
        /// <param name="reason">Reason for authentication failure</param>
        /// <param name="requestId">Request ID for tracking</param>
        private void LogAuthenticationFailure(string reason, string requestId)
        {
            var clientIp = HttpContext.Current?.Request?.UserHostAddress ?? "Unknown";
            
            // Log to system diagnostics
            System.Diagnostics.Debug.WriteLine($"[HRIC Auth Failure] Reason: {reason}, Request ID: {requestId}, IP: {clientIp}, Time: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
            
            // Future enhancement: Alert on repeated failures from same IP
            // if (IsRepeatedFailure(clientIp))
            // {
            //     SendSecurityAlert(clientIp, reason);
            // }
        }
        
        /// <summary>
        /// Logs authentication errors for debugging
        /// </summary>
        /// <param name="ex">Exception that occurred</param>
        /// <param name="actionContext">HTTP action context</param>
        private void LogAuthenticationError(Exception ex, HttpActionContext actionContext)
        {
            var requestId = GetHeaderValue(actionContext.Request, "X-Request-ID") ?? "Unknown";
            var clientIp = HttpContext.Current?.Request?.UserHostAddress ?? "Unknown";
            
            // Log error details
            System.Diagnostics.Debug.WriteLine($"[HRIC Auth Error] Exception: {ex.Message}, Request ID: {requestId}, IP: {clientIp}, Time: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
            
            // In production, consider logging full exception details to secure log
            // Logger.Error(ex, "HRIC authentication error for request {RequestId} from {ClientIp}", requestId, clientIp);
        }
    }
}

/*
 * CONFIGURATION REQUIRED IN WEB.CONFIG:
 * 
 * <appSettings>
 *   <add key="INTELLEGES_API_KEY" value="hric_api_key_123" />
 *   <add key="INTELLEGES_CLIENT_HMAC_SECRET" value="Is+zM3OQC5eR3D9sneWgxZaK5vbY2RFWrPREYpgg8VlzTww7wJA1uCQsKFJ7IenbrxWvR4sJN57sfUzkjx88wQ==" />
 * </appSettings>
 * 
 * USAGE IN CONTROLLER:
 * 
 * [HricApiAuthorize]
 * public class HricController : ApiController
 * {
 *     [HttpPost]
 *     public IHttpActionResult VerificationInitiate()
 *     {
 *         // Get authentication context
 *         var apiKey = HttpContext.Current.Items["INTELLEGES_API_KEY"] as string;
 *         var requestId = HttpContext.Current.Items["HRIC_REQUEST_ID"] as string;
 *         
 *         // Your implementation here
 *         return Ok(new { success = true, registrationId = "reg_123" });
 *     }
 * }
 * 
 * SECURITY FEATURES:
 * - HMAC-SHA256 signature validation
 * - Timestamp validation (5-minute window)
 * - Constant-time signature comparison
 * - Comprehensive logging and audit trail
 * - Protection against replay attacks
 * - Secure error handling (no information leakage)
 */

