using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Security.Cryptography;
using System.IO;
using System.Web;
using System.Text;
using System.Linq;

namespace GladCherryShopping.Helpers
{
    public static class FunctionsHelper
    {
        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int Keysize = 256;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;

        public static string Encrypt(string plainText, string passPhrase)
        {
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(string cipherText, string passPhrase)
        {
            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        public static bool ResizeImage(HttpPostedFileBase file, HttpServerUtilityBase Server, string path, int width, int height)
        {
            try
            {
                Stream Stream = file.InputStream;
                using (var TargetImage = System.Drawing.Image.FromStream(Stream))
                {
                    int NewWidth = width;
                    int NewHeight = height;
                    var thumbImage = new Bitmap(NewWidth, NewHeight);
                    var thumbGraph = Graphics.FromImage(thumbImage);
                    thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                    thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                    thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    var ImageRectangle = new Rectangle(0, 0, NewWidth, NewHeight);
                    thumbGraph.DrawImage(TargetImage, ImageRectangle);
                    thumbImage.Save(path, TargetImage.RawFormat);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static bool ResizeImage(System.Drawing.Image file, HttpServerUtilityBase Server, string path, string fileName, string extension, int width, int height)
        {
            try
            {
                using (var TargetImage = file)
                {
                    int NewWidth = width;
                    int NewHeight = height;
                    var thumbImage = new Bitmap(NewWidth, NewHeight);
                    var thumbGraph = Graphics.FromImage(thumbImage);
                    thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    var ImageRectangle = new Rectangle(0, 0, NewWidth, NewHeight);
                    thumbGraph.DrawImage(TargetImage, ImageRectangle);
                    thumbImage.Save(path + fileName + extension);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static string File(FileMode mode, string filePath, HttpServerUtilityBase Server)
        {
            if (mode == FileMode.Delete)
            {
                if (System.IO.File.Exists(Server.MapPath(filePath)))
                {
                    System.IO.File.Delete(Server.MapPath(filePath));
                    return "done";
                }
            }
            return string.Empty;
        }

        public static string File(FileMode mode, FileType type, string filePath, bool alterName, HttpPostedFileBase file, HttpServerUtilityBase Server)
        {
            if (mode != FileMode.Delete)
            {
                if (type == FileType.Image || type == FileType.Video || type == FileType.Sound)
                {
                    var FileExtension = Path.GetExtension(file.FileName);
                    if (FileExtension.ToLower() == ".jpg" || FileExtension.ToLower() == ".jpeg" || FileExtension.ToLower() == ".png" || FileExtension.ToLower() == ".webp" || FileExtension.ToLower() == ".gif" || FileExtension.ToLower() == ".wmv" || FileExtension.ToLower() == ".gif" || FileExtension.ToLower() == ".mp4" || FileExtension.ToLower() == ".gif" || FileExtension.ToLower() == ".ogg" || FileExtension.ToLower() == ".gif" || FileExtension.ToLower() == ".mpeg" || FileExtension.ToLower() == ".mp3")
                    {
                        var fileName = Path.GetFileName(file.FileName);

                        int LastIndex = LastIndex = filePath.LastIndexOf("/");

                        if (mode == FileMode.Update)
                        {
                            string directory;
                            if (!filePath.StartsWith("~"))
                            {
                                directory = "~" + filePath;
                                LastIndex = LastIndex + 1;
                            }

                            directory = filePath.Substring(0, LastIndex);

                            if (!System.IO.Directory.Exists(Server.MapPath(directory)))
                            {
                                System.IO.Directory.CreateDirectory(Server.MapPath(directory));
                            }
                        }
                        else
                        {
                            if (!System.IO.Directory.Exists(Server.MapPath(filePath)))
                            {
                                System.IO.Directory.CreateDirectory(Server.MapPath(filePath));
                            }
                        }

                        if (alterName == true)
                        {
                            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
                            fileNameWithoutExtension = Path.GetRandomFileName();
                            fileName = string.Format("{0}{1}", fileNameWithoutExtension, Path.GetExtension(file.FileName));
                        }

                        string path;


                        if (mode == FileMode.Update)
                        {
                            if (!filePath.StartsWith("~"))
                            {
                                filePath = "~" + filePath;
                            }

                            if (System.IO.File.Exists(Server.MapPath(filePath)))
                            {
                                System.IO.File.Delete(Server.MapPath(filePath));
                            }

                            path = Path.Combine(Server.MapPath(filePath.Substring(0, LastIndex)), fileName);
                            file.SaveAs(path);
                        }
                        else
                        {
                            path = Path.Combine(Server.MapPath(filePath), fileName);
                        }

                        file.SaveAs(path);
                        var finalPath = filePath.Remove(0, 1).Substring(0, LastIndex) + fileName;
                        return finalPath;
                    }
                    return string.Empty;
                }
            }
            return string.Empty;
        }


        public static string File(FileMode mode, FileType type, string filePath, bool alterName, HttpPostedFileBase file, HttpServerUtilityBase Server, int width, int height)
        {
            if (mode != FileMode.Delete)
            {
                if (type == FileType.Image && type == FileType.Video && type == FileType.Sound)
                {
                    var FileExtension = Path.GetExtension(file.FileName);
                    if (FileExtension.ToLower() == ".jpg" || FileExtension.ToLower() == ".jpeg" || FileExtension.ToLower() == ".png" || FileExtension.ToLower() == ".webp" || FileExtension.ToLower() == ".gif" || FileExtension.ToLower() == ".gif" || FileExtension.ToLower() == ".wmv" || FileExtension.ToLower() == ".gif" || FileExtension.ToLower() == ".mp4" || FileExtension.ToLower() == ".gif" || FileExtension.ToLower() == ".ogg" || FileExtension.ToLower() == ".gif" || FileExtension.ToLower() == ".mpeg" || FileExtension.ToLower() == ".mp3")
                    {
                        var fileName = Path.GetFileName(file.FileName);

                        int lastIndex = filePath.LastIndexOf("/");
                        var directory = Server.MapPath(filePath.Substring(0, lastIndex));

                        if (!System.IO.Directory.Exists(directory))
                        {
                            System.IO.Directory.CreateDirectory(directory);
                        }

                        if (alterName == true)
                        {
                            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
                            fileNameWithoutExtension = Path.GetRandomFileName();
                            fileName = string.Format("{0}{1}", fileNameWithoutExtension, Path.GetExtension(file.FileName));
                        }

                        var path = Path.Combine(Server.MapPath(filePath), fileName);

                        if (mode == FileMode.Update)
                        {
                            if (System.IO.File.Exists(Server.MapPath(filePath)))
                            {
                                System.IO.File.Delete(Server.MapPath(filePath));
                            }

                            path = Path.Combine(Server.MapPath(filePath.Substring(0, lastIndex)), fileName);
                        }

                        bool resizeStatus = ResizeImage(file: file, Server: Server, path: path, width: width, height: height);
                        if (resizeStatus == true)
                            return filePath.Substring(0, lastIndex) + "/" + fileName;
                        else
                            return string.Empty;
                    }
                    return string.Empty;
                }
            }
            return string.Empty;
        }

        public enum FileType
        {
            Image = 0,
            Video = 1,
            Sound = 3
        }

        public enum FileMode
        {
            Upload = 0,
            Update = 1,
            Delete = 2
        }

        //public class NewCategory
        //{
        //    public int Id { get; set; }
        //    public string Name { get; set; }
        //    public string CategoryTitle { get; set; }
        //}


        //public static SelectList GetGroupedItems(List<Category> items, object selectValue)
        //{
        //    List<NewCategory> newCategoryList = new List<NewCategory>();
        //    foreach (var current in items)
        //    {
        //        if (current.ParentId == null)
        //        {

        //            NewCategory item = new NewCategory() { Id = current.Id, Name = current.Title };
        //            newCategoryList.Add(item);
        //        }
        //        else
        //        {
        //            NewCategory item = new NewCategory() { Id = current.Id, Name = current.Title, CategoryTitle = current.HeadCategory.Title };
        //            newCategoryList.Add(item);
        //        }
        //    }
        //    return new SelectList(newCategoryList, "Id", "Name", "CategoryTitle", selectValue);
        //}

        //public static SelectList GetGroupedItems2(List<Category> items, object selectValue)
        //{
        //    List<NewCategory> newCategoryList = new List<NewCategory>();
        //    foreach (var current in items)
        //    {
        //        if (current.ParentId == null)
        //        {

        //            NewCategory item = new NewCategory() { Id = current.Id, Name = current.Title, CategoryTitle = current.Title };
        //            newCategoryList.Add(item);
        //        }
        //        else
        //        {
        //            NewCategory item = new NewCategory() { Id = current.Id, Name = current.Title, CategoryTitle = current.HeadCategory.Title };
        //            newCategoryList.Add(item);
        //        }
        //    }
        //    return new SelectList(newCategoryList, "Id", "Name", "CategoryTitle", selectValue);
        //}

        public static string Truncate(string input, int length)
        {
            if (input.Length <= length)
            {
                return input;
            }
            else
            {
                return input.Substring(0, length) + "...";
            }
        }


        /// <summary>
        /// Convert Unix time value to a DateTime object.
        /// </summary>
        /// <param name="unixtime">The Unix time stamp you want to convert to DateTime.</param>
        /// <returns>Returns a DateTime object that represents value of the Unix time.</returns>
        public static DateTime UnixTimeToDateTime(long unixtime)
        {
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return sTime.AddSeconds(unixtime);
        }

        /// <summary>
        /// Convert a date time object to Unix time representation.
        /// </summary>
        /// <param name="datetime">The datetime object to convert to Unix time stamp.</param>
        /// <returns>Returns a numerical representation (Unix time) of the DateTime object.</returns>
        public static long ConvertToUnixTime(DateTime datetime)
        {
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return (long)(datetime - sTime).TotalSeconds;
        }


        public static string GetPersianDateTime(DateTime dateTime, bool isLongTime, bool isPersianMonth)
        {
            PersianCalendar persianCalendar = new PersianCalendar();
            int year = persianCalendar.GetYear(dateTime);
            int month = persianCalendar.GetMonth(dateTime);
            int day = persianCalendar.GetDayOfMonth(dateTime);
            int hour = persianCalendar.GetHour(dateTime);
            int minute = persianCalendar.GetMinute(dateTime);
            int second = persianCalendar.GetSecond(dateTime);
            string persianDateFormat = isLongTime ? string.Format("{0}/{1}/{2} - {3}:{4}:{5}", year, month, day, hour, minute, second)
                                                       : string.Format("{0}/{1}/{2}", year, month, day);
            if (isPersianMonth)
            {
                string persianMonth = "";
                switch (month)
                {
                    case 1:
                        persianMonth = "فروردین";
                        break;
                    case 2:
                        persianMonth = "اردیبهشت";
                        break;
                    case 3:
                        persianMonth = "خرداد";
                        break;
                    case 4:
                        persianMonth = "تیر";
                        break;
                    case 5:
                        persianMonth = "مرداد";
                        break;
                    case 6:
                        persianMonth = "شهریور";
                        break;
                    case 7:
                        persianMonth = "مهر";
                        break;
                    case 8:
                        persianMonth = "آبان";
                        break;
                    case 9:
                        persianMonth = "آذر";
                        break;
                    case 10:
                        persianMonth = "دی";
                        break;
                    case 11:
                        persianMonth = "بهمن";
                        break;
                    case 12:
                        persianMonth = "اسفند";
                        break;
                    default:
                        break;
                }
                persianDateFormat = isLongTime ? string.Format("{0} {1} {2} - {3}:{4}:{5}", day, persianMonth, year, hour, minute, second)
                                            : string.Format("{0} {1} {2}", day, persianMonth, year);
            }
            return persianDateFormat;

        }

        public static DateTime ConvertToGregorian(string date)
        {
            int[] StartDateToArray = date.Split('/').Select(id => Convert.ToInt32(id)).ToArray();
            int year = StartDateToArray[0];
            int month = StartDateToArray[1];
            int day = StartDateToArray[2];
            DateTime dt = new DateTime(year, month, day, new PersianCalendar());
            GregorianCalendar persianCalendar = new GregorianCalendar();
            int GregorianYear = persianCalendar.GetYear(dt);
            int GregorianMonth = persianCalendar.GetMonth(dt);
            int GregorianDay = persianCalendar.GetDayOfMonth(dt);
            string GreGorian = string.Format("{0}/{1}/{2}", GregorianYear, GregorianMonth, GregorianDay);
            return Convert.ToDateTime(GreGorian);
        }

        #region TimeCalculator
        const int SECOND = 1;
        const int MINUTE = 60 * SECOND;
        const int HOUR = 60 * MINUTE;
        const int DAY = 24 * HOUR;
        const int MONTH = 30 * DAY;
        /// <summary>
        /// محاصبه فاصله زمانی
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string Calculate(this DateTime dateTime)
        {
            var ts = new TimeSpan(DateTime.Now.Ticks - dateTime.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);
            if (delta < 1 * MINUTE)
            {
                return ts.Seconds == 1 ? "لحظه ای قبل" : ts.Seconds + " ثانیه قبل";
            }
            if (delta < 2 * MINUTE)
            {
                return "یک دقیقه قبل";
            }
            if (delta < 45 * MINUTE)
            {
                return ts.Minutes + " دقیقه قبل";
            }
            if (delta < 90 * MINUTE)
            {
                return "یک ساعت قبل";
            }
            if (delta < 24 * HOUR)
            {
                return ts.Hours + " ساعت قبل";
            }
            if (delta < 48 * HOUR)
            {
                return "دیروز";
            }
            if (delta < 30 * DAY)
            {
                return ts.Days + " روز قبل";
            }
            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "یک ماه قبل" : months + " ماه قبل";
            }
            int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "یک سال قبل" : years + " سال قبل";
        }
        #endregion TimeCalculator


        public static string GetPersianCurrency(string value)
        {
            try
            {
                CultureInfo persianCulture = new CultureInfo("fa-Ir");
                persianCulture.NumberFormat.CurrencyPositivePattern = 3;
                persianCulture.NumberFormat.CurrencyNegativePattern = 3;
                persianCulture.NumberFormat.CurrencySymbol = "ریال";
                long longValue = Convert.ToInt64(value);
                string persianCurrency = longValue.ToString("C0", persianCulture);
                return persianCurrency;
            }
            catch (Exception)
            {
                throw;
            }

        }


        public static string SerializeSefUrl(string text)
        {
            string trimText = text.Trim();
            string serializedText;
            if (trimText.Contains("/"))
            {
                serializedText = trimText.Replace("/", string.Empty);
            }
            else
            {
                serializedText = trimText;
            }

            if (trimText.Contains(":"))
            {
                serializedText = serializedText.Replace(":", string.Empty);
            }
            if (serializedText.Contains(";"))
            {
                serializedText = serializedText.Replace(";", string.Empty);
            }
            if (serializedText.Contains("+"))
            {
                serializedText = serializedText.Replace("+", string.Empty);
            }
            if (serializedText.Contains("."))
            {
                serializedText = serializedText.Replace(".", string.Empty);
            }
            if (serializedText.Contains("?"))
            {
                serializedText = serializedText.Replace("?", string.Empty);
            }
            if (serializedText.Contains("#"))
            {
                serializedText = serializedText.Replace("#", string.Empty);
            }
            serializedText = serializedText.Replace(" ", "-");
            if (serializedText.Contains("--"))
            {
                serializedText = serializedText.Replace("--", "-");
            }

            return serializedText;
        }

        public static string ToPersianNumber(string input)
        {
            string[] persian = new string[10] { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };

            for (int j = 0; j < persian.Length; j++)
                input = input.Replace(j.ToString(), persian[j]);

            return input;
        }

        public static System.Drawing.Image AddWaterMark(HttpServerUtilityBase server, string imagePath, string watermarkPath, string saveFilePath, string fileName, string extension)
        {
            //using (System.Drawing.Image image = System.Drawing.Image.FromFile(server.MapPath(imagePath)))
            //{
            System.Drawing.Image image = System.Drawing.Image.FromFile(server.MapPath(imagePath));
            using (System.Drawing.Image watermark = System.Drawing.Image.FromFile(server.MapPath(watermarkPath)))
            {
                using (Graphics g = Graphics.FromImage(image))
                {
                    int x = image.Width - watermark.Width;
                    int y = image.Height - watermark.Height;
                    g.DrawImage(watermark, new Point(x, y));
                    image.Save(server.MapPath(saveFilePath + fileName + extension));
                }
                return image;
            }
            //}
        }

        public static string CreateThumbnail(int maxWidth, int maxHeight, string path, HttpServerUtilityBase server)
        {

            if (!Directory.Exists("~/Content/Images/minify/"))
            {
                Directory.CreateDirectory(server.MapPath("~/Content/Images/minify/"));
            }
            var image = System.Drawing.Image.FromFile(path);
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);
            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);
            var newImage = new Bitmap(newWidth, newHeight);
            Graphics thumbGraph = Graphics.FromImage(newImage);
            thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
            thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
            thumbGraph.DrawImage(image, 0, 0, newWidth, newHeight);
            image.Dispose();
            string relativeFilePath = "/Content/Images/minify/" + maxWidth + Path.GetFileName(path);
            if (System.IO.File.Exists(server.MapPath(relativeFilePath)))
            {
                return relativeFilePath;
            }
            newImage.Save(server.MapPath(relativeFilePath), newImage.RawFormat);
            return relativeFilePath;
        }
    }


}