using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;


namespace BSFinancial.Services
{
    public class PDFService
    {
        /// <summary>
        /// Set Cell Properties
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="colspan"></param>
        /// <param name="align"></param>
        /// <param name="fontSize"></param>
        /// <param name="FontStyle"></param>
        /// <param name="border"></param>
        /// <param name="borderWidth"></param>
        /// <returns></returns>
        private PdfPCell SetCellProperties(String Text, Int32 colspan, Int32 align, float fontSize, Int32 FontStyle, Int32 border, float borderWidth)
        {
            PdfPCell cell = SetCellProperties(Text, colspan, align, fontSize, FontStyle);
            //cell.BorderColor = new BaseColor(System.Drawing.Color.Red);
            cell.Border = border; //iTextSharp.text.Rectangle.NO_BORDER; // | Rectangle.TOP_BORDER;
            cell.BorderWidthBottom = borderWidth;
            return cell;
        }

        private PdfPCell SetCellProperties(String Text, Int32 colspan, Int32 rowspan, Int32 align, float fontSize, Int32 FontStyle)
        {
            PdfPCell cell = SetCellProperties(Text, colspan, align, fontSize, FontStyle);
            cell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            cell.Rowspan = rowspan;
            return cell;
        }

        private PdfPCell SetCellProperties(String Text, Int32 colspan, Int32 align, float fontSize, Int32 FontStyle)
        {

            Font fontArial = new Font(FontFactory.GetFont("Arial", fontSize, FontStyle));

            //Font Style 0= Normal; 1=Bold
            //Align 0=Left, 1=Centre, 2=Right
            PdfPCell cell = new PdfPCell(new Phrase(Text, fontArial));
            //cell.Colspan = 3;
            cell.HorizontalAlignment = align;
            ////Style
            cell.Colspan = colspan;
            return cell;
        }

        /// <summary>
        /// Set Color to the cell
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="colspan"></param>
        /// <param name="align"></param>
        /// <param name="fontSize"></param>
        /// <param name="FontStyle"></param>
        /// <param name="border"></param>
        /// <param name="borderWidth"></param>
        /// <returns></returns>
        private PdfPCell SetCellPropertiesWithColor(String Text, Int32 colspan, Int32 align, float fontSize, Int32 FontStyle, Int32 border, float borderWidth)
        {

            Font fontArial = new Font(FontFactory.GetFont("Arial", fontSize, FontStyle, new BaseColor(System.Drawing.Color.OrangeRed)));
            //Font Style 0= Normal; 1=Bold
            //Align 0=Left, 1=Centre, 2=Right
            PdfPCell cell = new PdfPCell(new Phrase(Text, fontArial));
            //cell.Colspan = 3;
            cell.HorizontalAlignment = align;
            ////Style
            cell.Colspan = colspan;
            //cell.BorderColor = new BaseColor(System.Drawing.Color.Red);
            cell.Border = border; //iTextSharp.text.Rectangle.NO_BORDER; // | Rectangle.TOP_BORDER;
            cell.BorderWidthBottom = borderWidth;
            return cell;
        }

        /// Return Decimal value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String DecimalValue(String value)
        {
            String _numb = String.Empty;
            if (!String.IsNullOrEmpty(value))
                _numb = Convert.ToDecimal(value).ToString("0.00");
            else
                _numb = value;
            return _numb;
        }

        public static String CurrencyFormat(Decimal value)
        {
            return Convert.ToDecimal(value).ToString("C2");
        }

        public static String breakLine(int line)
        {
            string br = "\r\n";
            for (int i = 0; i < line; i++)
            {
                string newbr = "\r\n";
                br = br + newbr;
            }
            return br;
        }
       
        public byte[] GeneratePDF(BSFinancial.Data.Loan loan)
        {
            FontFactory.RegisterDirectories();

            //var pageSize = new iTextSharp.text.Rectangle(640f, 800F);
            Document doc = new Document(PageSize.A4, 30f, 30f, 40f, 40f);
            byte[] pdfBytes;

            try
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    PdfWriter writer = PdfWriter.GetInstance(doc, memStream);
                    //PdfWriter writer = PdfWriter.GetInstance(doc, filepath);
                    // calling PageEventHelper class to Include in document
                    PageEventHelper pageEventHelper = new PageEventHelper();
                    writer.PageEvent = pageEventHelper;
                    doc.Open();

                    #region Header Section
                    float[] widths = new float[] { 100f };
                    PdfPTable tabheader = new PdfPTable(widths);
                    tabheader.WidthPercentage = 100.0f;
                    tabheader.DefaultCell.Border = Rectangle.NO_BORDER;

                    tabheader.AddCell(SetCellProperties("Statement", 0, 1, 14f, 1, 2, 1f));
                    tabheader.AddCell(SetCellProperties(breakLine(1), 0, 1, 8f, 1, 0, 0));

                    tabheader.AddCell(SetCellProperties("Loan Details", 0, 0, 12f, 1, 2, 0.5f));
                    tabheader.AddCell(SetCellProperties(breakLine(1), 0, 1, 8f, 1, 0, 0));

                    // Tab loan Info
                    string username = loan.application.MainApplicant.FirstName + " " + loan.application.MainApplicant.LastName;
                    tabheader.AddCell(SetCellProperties("Name: " + username, 0, 0, 10f, 1, 0, 0));
                    if (!String.IsNullOrEmpty(loan.AccountNumber))
                        tabheader.AddCell(SetCellProperties("Loan Account Number:" + loan.AccountNumber, 0, 0, 10f, 0, 0, 0));
                    string address1 = string.Empty;
                    string address2 = string.Empty;
                    string zipcode = string.Empty;
                    string phone = string.Empty;
                    if (loan.application != null && loan.application.Addrs.Count > 0)
                    {
                        address1 = loan.application.Addrs.FirstOrDefault().Street;
                        address2 = loan.application.Addrs.FirstOrDefault().City + ", " + loan.application.Addrs.FirstOrDefault().State;
                        zipcode = loan.application.Addrs.FirstOrDefault().ZipCode.ToString();
                        phone = loan.application.MainApplicant.Mobile;
                    }
                    if (!String.IsNullOrEmpty(address1))
                        tabheader.AddCell(SetCellProperties("Address:" + address1, 0, 0, 10f, 1, 0, 0));
                    if (!String.IsNullOrEmpty(address2))
                        tabheader.AddCell(SetCellProperties(address2, 0, 0, 10f, 1, 0, 0));
                    if (!String.IsNullOrEmpty(phone))
                        tabheader.AddCell(SetCellProperties("Phone: " + phone, 0, 0, 10f, 1, 0, 0));
                   
                   
                    tabheader.AddCell(SetCellProperties(breakLine(1), 0, 1, 8f, 0, 0, 0f));
                    tabheader.AddCell(SetCellProperties("Principal Amount: " + CurrencyFormat(loan.Principal), 0, 0, 10f, 0, 0, 0));
                    tabheader.AddCell(SetCellProperties("Begin Date: " + loan.BeginDate.ToShortDateString(), 0, 0, 10f, 0, 0, 0));
                    tabheader.AddCell(SetCellProperties("Maturity Date: " + loan.MaturityDate.ToShortDateString(), 0, 0, 10f, 0, 0, 0));
                   // tabheader.AddCell(SetCellProperties("Rate of Interest : " + loan.Rate, 0, 0, 10f, 0, 0, 0));
                    decimal remainingbal = loan.Principal - loan.Payments.Sum(p=>p.PrincipalAmt);
                    tabheader.AddCell(SetCellProperties("Principal Balance: " + CurrencyFormat(remainingbal), 2, 0, 10f, 0, 0, 0));
                    tabheader.AddCell(SetCellProperties(breakLine(1), 0, 1, 8f, 1, 0, 0));

                    // add to doc
                    doc.Add(tabheader);
                    #endregion


                    #region Payments  List
                    float[] paywidths = new float[] { 100f };
                    PdfPTable paymentheader = new PdfPTable(paywidths);
                    paymentheader.WidthPercentage = 100.0f;
                    paymentheader.DefaultCell.Border = Rectangle.NO_BORDER;

                    paymentheader.AddCell(SetCellProperties("Payments", 0, 0, 12f, 1, 2, 0.5f));
                    paymentheader.AddCell(SetCellProperties(breakLine(1), 0, 1, 8f, 1, 0, 0));
                    doc.Add(paymentheader);


                    widths = new float[] { 20, 20, 15, 15, 20 };
                    PdfPTable tabOrderItemInfo = new PdfPTable(widths);
                    tabOrderItemInfo.WidthPercentage = 95.0f;
                    tabOrderItemInfo.DefaultCell.Border = Rectangle.NO_BORDER;

                    //tabOrderItemInfo.AddCell(SetCellProperties("Payments are listed below :", 6, 0, 12f, 0, 0, 0));
                    // Heading
                    //tabOrderItemInfo.AddCell(SetCellProperties("#Id", 1, 1, 12f, 1));
                    tabOrderItemInfo.AddCell(SetCellProperties("Paid On", 1, 1, 10f, 1));
                    tabOrderItemInfo.AddCell(SetCellProperties("Principal", 1, 1, 10f, 1));
                    tabOrderItemInfo.AddCell(SetCellProperties("Interest", 1, 1, 10f, 1));
                    tabOrderItemInfo.AddCell(SetCellProperties("Escrow", 1, 1, 10f, 1));
                    tabOrderItemInfo.AddCell(SetCellProperties("Total Paid", 1, 1, 10f, 1));

                    decimal total = 0;
                    decimal prin = 0;
                    decimal interest = 0;
                    decimal escrow = 0;
                    foreach (var item in loan.Payments)
                    {
                        //tabOrderItemInfo.AddCell(SetCellProperties(item.Id.ToString(), 1, 1, 1, 12f, 0));
                        tabOrderItemInfo.AddCell(SetCellProperties(item.PayDate.ToShortDateString(), 1, 1, 0, 10f, 0));
                        tabOrderItemInfo.AddCell(SetCellProperties(CurrencyFormat(item.PrincipalAmt), 1, 1, 2, 10f, 0));
                        prin += item.PrincipalAmt;
                        tabOrderItemInfo.AddCell(SetCellProperties(CurrencyFormat(item.InterestAmt), 1, 1, 2, 10f, 0));
                        interest += item.InterestAmt;
                        tabOrderItemInfo.AddCell(SetCellProperties(CurrencyFormat(item.EscrowAmt), 1, 1, 2, 10f, 0));
                        escrow += item.EscrowAmt;
                        tabOrderItemInfo.AddCell(SetCellProperties(CurrencyFormat(item.TotalAmt), 1, 1, 2, 10f, 0));
                        total += item.TotalAmt;
                    }

                    tabOrderItemInfo.AddCell(SetCellProperties("Totals", 1, 1, 1f, 0, 0, 0));
                    tabOrderItemInfo.AddCell(SetCellProperties(CurrencyFormat(prin), 1, 1, 2, 10f, 0));
                    tabOrderItemInfo.AddCell(SetCellProperties(CurrencyFormat(interest), 1, 1, 2, 10f, 0));
                    tabOrderItemInfo.AddCell(SetCellProperties(CurrencyFormat(escrow), 1, 1, 2, 10f, 0));
                    tabOrderItemInfo.AddCell(SetCellProperties(CurrencyFormat(total), 1, 1, 2, 10f, 0));

                    tabOrderItemInfo.AddCell(SetCellProperties(String.Empty, 6, 1, 1f, 0, 0, 0));

                    doc.Add(tabOrderItemInfo);


                    #endregion
                    #region Footer Padding

                    // add footer
                    //PdfPTable tabFooterSectoin = new PdfPTable(1);
                    //tabFooterSectoin.WidthPercentage = 100.0f;
                    //tabFooterSectoin.DefaultCell.Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                    // doc.Add(tabFooterSectoin);
                    #endregion
                    doc.Close();
                    pdfBytes = memStream.ToArray();
                    memStream.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return pdfBytes;
        }
    }

    public class PageEventHelper : PdfPageEventHelper
    {
        PdfContentByte cb;
        PdfTemplate template;
        // this is the BaseFont we are going to use for the header / footer
        BaseFont bf = null;
        // we override the onOpenDocument method
        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            try
            {
                //PrintTime = DateTime.Now;
                bf = BaseFont.CreateFont();//BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;
                template = cb.CreateTemplate(50, 50);
            }
            catch (DocumentException de)
            {
            }
            catch (System.IO.IOException ioe)
            {
            }
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            int pageN = writer.PageNumber;
            String text = pageN + "/";//"Page " + pageN + " of ";
            float len = bf.GetWidthPoint(text, 8);

            Rectangle pageSize = document.PageSize;

            cb.SetRGBColorFill(100, 100, 100);

            cb.BeginText();
            cb.SetFontAndSize(bf, 8);
            cb.SetTextMatrix(299, pageSize.GetBottom(30));//pageSize.GetLeft(40),pageSize.GetBottom(30));
            cb.ShowText(text);
            cb.EndText();

            cb.AddTemplate(template, 299 + len, pageSize.GetBottom(30));


        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);

            template.BeginText();
            template.SetFontAndSize(bf, 8);
            template.SetTextMatrix(0, 0);
            template.ShowText("" + (writer.PageNumber - 1));
            template.EndText();
        }


    }
}