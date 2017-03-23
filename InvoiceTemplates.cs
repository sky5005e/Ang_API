using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;


namespace CreatePDF
{
    public class InvoiceTemplates
    {
        public InvoiceTemplates()
        {
            //
            // TODO: Add constructor logic here
            //
        }


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



        public void GenerateInvoicePDF(String filePath, OrderDetails objOrder)
        {
            FontFactory.RegisterDirectories();


            var pageSize = new iTextSharp.text.Rectangle(640f, 800F);
            Document doc = new Document(PageSize.A4, 30f, 30f, 40f, 40f);
            //byte[] pdfBytes;

            MemoryStream memStream = new MemoryStream();
            //PdfWriter writer = PdfWriter.GetInstance(doc, memStream);
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
            // calling PageEventHelper class to Include in document
            PageEventHelper pageEventHelper = new PageEventHelper();
            writer.PageEvent = pageEventHelper;
            doc.Open();
            try
            {
                // 1 f = 1.71Pixel
                #region Header Section
                float[] widths = new float[] { 50, 50 };
                PdfPTable tabheader = new PdfPTable(widths);
                tabheader.WidthPercentage = 100.0f;
                tabheader.DefaultCell.Border = 0;// No Border
                //tabheader.
               // iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(@"C:\Users\Sky\Desktop\All_Pics\SkyIncLogo.png");
               // //Resize image depend upon your need
               // jpg.ScaleToFit(40f, 40f);
               //// Give space before image
               //  jpg.SpacingBefore = 9f;
               // //Give some space after the image
               //  jpg.SpacingAfter = 1f;
               //  jpg.Alignment = Element.ALIGN_LEFT;

               //  tabheader.AddCell(jpg);
                tabheader.AddCell(SetCellProperties("INVOICE", 2, 1, 14f, 1, 0, 0));
                // Tab Company Info
                PdfPTable tableftCinfo = new PdfPTable(1);
                tableftCinfo.AddCell(SetCellProperties("Adeeva Nutritionals", 0, 0, 10f, 1, 0, 0));
                tableftCinfo.AddCell(SetCellProperties("5500 Explorer Drive, 4th Floor,", 0, 0, 10f, 0, 0, 0));
                tableftCinfo.AddCell(SetCellProperties("Mississauga, ON L4W 5C7", 0, 0, 10f, 0, 0, 0));
                tableftCinfo.AddCell(SetCellProperties("Canada", 0, 0, 10f, 0, 0, 0));
                tableftCinfo.AddCell(SetCellProperties("Phone : 1-888-494-1010 , 1-888-251-1010", 0, 0, 10f, 0, 0, 0));
                tableftCinfo.AddCell(SetCellProperties("", 0, 0, 10f, 0, 0, 0));
                // Tab Company Info
                widths = new float[] { 21, 29 };
                PdfPTable tabRightCinfo = new PdfPTable(widths);
                tabRightCinfo.AddCell(SetCellProperties(String.Empty, 2, 2, 10f, 1, 0, 0));
                tabRightCinfo.AddCell(SetCellProperties("DATE : ", 0, 2, 10f, 0, 0, 0));
                var ORDDate = DateTime.ParseExact(objOrder.INVOrderDate, "yyyyMMdd", null);
                tabRightCinfo.AddCell(SetCellProperties(ORDDate.ToShortDateString(), 0, 0, 10f, 0, 0, 0));
                tabRightCinfo.AddCell(SetCellProperties("INVOICE NUMBER : ", 0, 2, 10f, 0, 0, 0));
                tabRightCinfo.AddCell(SetCellProperties(objOrder.INVNumber, 0, 0, 10f, 0, 0, 0));
                tabRightCinfo.AddCell(SetCellProperties("CUSTOMER NO. : ", 0, 2, 10f, 0, 0, 0));
                tabRightCinfo.AddCell(SetCellProperties(objOrder.INVCustomer, 0, 0, 10f, 0, 0, 0));
                // Row 1
                tabheader.AddCell(tableftCinfo);
                tabheader.AddCell(tabRightCinfo);
                tabheader.AddCell(SetCellProperties(String.Empty, 2, 1, 8f, 0, 0, 0f));
                // add to doc
                doc.Add(tabheader);

                //row 2
                // Tab Bill to Info
                PdfPTable tabBillingShippingInfo = new PdfPTable(2);
                tabBillingShippingInfo.WidthPercentage = 100.0f;
                tabBillingShippingInfo.DefaultCell.BorderWidth = 1f;
                tabBillingShippingInfo.DefaultCell.Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                // title
                tabBillingShippingInfo.AddCell(SetCellProperties("BILL TO :", 0, 0, 10f, 1));
                tabBillingShippingInfo.AddCell(SetCellProperties("SHIP TO :", 0, 0, 10f, 1));
                // User Name
                tabBillingShippingInfo.AddCell(SetCellProperties(objOrder.BilName, 0, 0, 10f, 0));
                tabBillingShippingInfo.AddCell(SetCellProperties(objOrder.ShipName, 0, 0, 10f, 0));

                tabBillingShippingInfo.AddCell(SetCellProperties(objOrder.BilAddr1 + ", " + objOrder.BilAddr2, 0, 0, 10f, 0));
                tabBillingShippingInfo.AddCell(SetCellProperties(objOrder.ShipAddr1 + ", " + objOrder.ShipAddr2, 0, 0, 10f, 0));

                //tabBillingShippingInfo.AddCell(SetCellProperties(objOrder.BilAddr3 + ", " + objOrder.BilAddr4, 0, 0, 10f, 0));
                //tabBillingShippingInfo.AddCell(SetCellProperties(objOrder.ShipAddr3 + ", " + objOrder.ShipAddr4, 0, 0, 10f, 0));

                tabBillingShippingInfo.AddCell(SetCellProperties(objOrder.BilCity + ", " + objOrder.BilState + ", " + objOrder.BilZip, 0, 0, 10f, 0));
                tabBillingShippingInfo.AddCell(SetCellProperties(objOrder.ShipCity + ", " + objOrder.ShipState + ", " + objOrder.ShipZip, 0, 0, 10f, 0));

                tabBillingShippingInfo.AddCell(SetCellProperties(objOrder.BilCountry, 0, 0, 10f, 0));
                tabBillingShippingInfo.AddCell(SetCellProperties(objOrder.ShipCountry, 0, 0, 10f, 0));

                tabBillingShippingInfo.AddCell(SetCellProperties(objOrder.BilPhone, 0, 0, 10f, 0));
                tabBillingShippingInfo.AddCell(SetCellProperties(objOrder.ShipPhone, 0, 0, 10f, 0));

                tabBillingShippingInfo.AddCell(SetCellProperties("", 0, 0, 10f, 0));

                tabBillingShippingInfo.AddCell(SetCellProperties("Instruction: " + objOrder.INVDesc + " " + objOrder.INVDeliveryIns, 0, 0, 10f, 0));
                // For space
                tabBillingShippingInfo.AddCell(SetCellProperties(String.Empty, 2, 1, 1f, 0, 0, 0));
                tabBillingShippingInfo.AddCell(SetCellProperties(String.Empty, 2, 1, 1f, 0, 0, 0));
                // add to doc
                doc.Add(tabBillingShippingInfo);

                // 
                widths = new float[] { 15, 30, 19, 16, 20 };
                PdfPTable tabOrderInfo = new PdfPTable(widths);
                tabOrderInfo.WidthPercentage = 100.0f;
                tabOrderInfo.DefaultCell.BorderWidth = 1f;
                tabOrderInfo.DefaultCell.Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                tabOrderInfo.AddCell(SetCellProperties("PO NUMBER", 0, 1, 12f, 1));
                tabOrderInfo.AddCell(SetCellProperties("SPECIAL INSTRUCTIONS", 0, 1, 12f, 1));
                tabOrderInfo.AddCell(SetCellProperties("SALES PERSON", 0, 1, 12f, 1));
                tabOrderInfo.AddCell(SetCellProperties("ORDER DATE", 0, 1, 12f, 1));
                tabOrderInfo.AddCell(SetCellProperties("ORDER NUMBER", 0, 1, 12f, 1));

                tabOrderInfo.AddCell(SetCellProperties("-", 0, 1, 12f, 0));
                tabOrderInfo.AddCell(SetCellProperties(objOrder.INVDesc + " " + objOrder.INVDeliveryIns, 0, 1, 12f, 0));
                tabOrderInfo.AddCell(SetCellProperties("-", 0, 1, 12f, 0));
                tabOrderInfo.AddCell(SetCellProperties(ORDDate.ToShortDateString(), 0, 1, 12f, 0));
                tabOrderInfo.AddCell(SetCellProperties(objOrder.INVOrderNumber, 0, 1, 12f, 0));

                tabOrderInfo.AddCell(SetCellProperties(String.Empty, 5, 1, 1f, 0, 0, 0));
                tabOrderInfo.AddCell(SetCellProperties(String.Empty, 5, 1, 1f, 0, 0, 0));

                doc.Add(tabOrderInfo);

                PdfPTable tabShippingInfo = new PdfPTable(2);
                tabShippingInfo.WidthPercentage = 100.0f;
                tabShippingInfo.DefaultCell.BorderWidth = 1f;
                tabShippingInfo.DefaultCell.Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                // title
                tabShippingInfo.AddCell(SetCellProperties("SHIP VIA", 0, 1, 10f, 1));
                tabShippingInfo.AddCell(SetCellProperties("TERMS", 0, 1, 10f, 1));
                // User Name
                tabShippingInfo.AddCell(SetCellProperties(objOrder.INVViaDesc == "" ? "-" : objOrder.INVViaDesc, 0, 1, 10f, 0));
                tabShippingInfo.AddCell(SetCellProperties(" ", 0, 1, 10f, 0));

                tabShippingInfo.AddCell(SetCellProperties(String.Empty, 2, 1, 1f, 0, 0, 0));
                tabShippingInfo.AddCell(SetCellProperties(String.Empty, 2, 1, 1f, 0, 0, 0));

                doc.Add(tabShippingInfo);

                #endregion
                #region Order Items
                widths = new float[] { 40, 13, 14, 9, 10, 14 };
                PdfPTable tabOrderItemInfo = new PdfPTable(widths);
                tabOrderItemInfo.WidthPercentage = 100.0f;
                tabOrderItemInfo.DefaultCell.BorderWidth = 1f;
                tabOrderItemInfo.DefaultCell.Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;

                // Heading
                tabOrderItemInfo.AddCell(SetCellProperties("PART NUMBER DESCRIPTION", 1, 2, 1, 12f, 1));
                tabOrderItemInfo.AddCell(SetCellProperties("QUANTITY", 3, 1, 1, 12f, 1));
                tabOrderItemInfo.AddCell(SetCellProperties("UNIT PRICE", 1, 2, 1, 12f, 1));
                tabOrderItemInfo.AddCell(SetCellProperties("EXTENDED PRICE", 1, 2, 1, 12f, 1));
                // new heading
                tabOrderItemInfo.AddCell(SetCellProperties("REQ.", 0, 1, 12f, 0, 0, 0f));
                tabOrderItemInfo.AddCell(SetCellProperties("SHIPPED", 0, 1, 12f, 0, Rectangle.LEFT_BORDER, 0f));
                tabOrderItemInfo.AddCell(SetCellProperties("B.O.", 0, 1, 12f, 0, Rectangle.LEFT_BORDER, 0f));


                foreach (var item in objOrder.OrderItems)
                {
                    tabOrderItemInfo.AddCell(SetCellProperties(item.ItemPartNo + " - " + item.ItemDesc, 1, 1, 0, 12f, 0));
                    tabOrderItemInfo.AddCell(SetCellProperties(CommonCls.IntValue(item.ItemQtyOrdered), 1, 1, 1, 12f, 0));
                    tabOrderItemInfo.AddCell(SetCellProperties(CommonCls.IntValue(item.ItemShipped), 1, 1, 1, 12f, 0));
                    tabOrderItemInfo.AddCell(SetCellProperties(CommonCls.IntValue(item.ItemQtyBackOrdered), 1, 1, 1, 12f, 0));
                    tabOrderItemInfo.AddCell(SetCellProperties(CommonCls.DecimalValue(item.ItemUnitPrice), 1, 1, 2, 12f, 0));

                    tabOrderItemInfo.AddCell(SetCellProperties(CommonCls.DecimalValue(item.ItemInvMisc), 1, 1, 2, 12f, 0));
                }

                // footer order items
                tabOrderItemInfo.AddCell(SetCellProperties("Subtotal :", 5, 2, 12f, 0));
                tabOrderItemInfo.AddCell(SetCellProperties(CommonCls.DecimalValue(objOrder.INVTotalAmount), 0, 2, 12f, 0));

                tabOrderItemInfo.AddCell(SetCellProperties("Less Discount :", 5, 2, 12f, 0));
                tabOrderItemInfo.AddCell(SetCellProperties(CommonCls.DecimalValue(objOrder.INVDiscAmount), 0, 2, 12f, 0));

                tabOrderItemInfo.AddCell(SetCellProperties("Misc. Charge(Shipping) :", 5, 2, 12f, 0));
                tabOrderItemInfo.AddCell(SetCellProperties(CommonCls.DecimalValue(objOrder.INVMisc), 0, 2, 12f, 0));

                tabOrderItemInfo.AddCell(SetCellProperties("Total Sales Tax :", 5, 2, 12f, 0));
                tabOrderItemInfo.AddCell(SetCellProperties(CommonCls.DecimalValue(objOrder.INVTaxTotal), 0, 2, 12f, 0));

                tabOrderItemInfo.AddCell(SetCellProperties("Total amount :", 5, 2, 12f, 0));
                tabOrderItemInfo.AddCell(SetCellProperties(CommonCls.DecimalValue(objOrder.INVNetTax), 0, 2, 12f, 0));

                tabOrderItemInfo.AddCell(SetCellProperties(String.Empty, 6, 1, 1f, 0, 0, 0));
                tabOrderItemInfo.AddCell(SetCellProperties(String.Empty, 6, 1, 1f, 0, 0, 0));

                doc.Add(tabOrderItemInfo);

                #endregion
                #region Footer Padding

                // add footer
                PdfPTable tabFooterSectoin = new PdfPTable(1);
                tabFooterSectoin.WidthPercentage = 100.0f;
                tabFooterSectoin.DefaultCell.Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                tabFooterSectoin.AddCell(SetCellProperties("Comments:", 0, 0, 9f, 2));
                tabFooterSectoin.AddCell(SetCellProperties("Adeeva Nutritionals Canada Inc. Will give a full refund or exchange on product purchased at regular price within 30 days of purchase. Product purchased on sale will not be refunded or exchanged. To return items. please call 1-877-264-8080 for a Return Authorization Number", 0, 0, 9f, 2));

                tabFooterSectoin.AddCell(SetCellProperties("HST#:89646 4393", 0, 0, 9f, 1));
                doc.Add(tabFooterSectoin);
                #endregion

                //pdfBytes = memStream.ToArray();
            }

            catch (Exception ex)
            {
                CommonCls.ErrorMessage(ex, @"D:\CRM\CRM\WWWRoot\CustomPages\OrderTracking\CatalystXML\" + "file" + DateTime.Now.Ticks + ".txt");
                throw ex;
            }

            finally
            {
                writer.CloseStream = true;
                doc.Close();

            }

            //return pdfBytes;
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
