using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Web.Helpers;

namespace MMS.DAL
{

    public class HeaderFooter : PdfPageEventHelper
    {
        string  footerText = "";
        public HeaderFooter(string header,string footer)
        {
            footerText = footer;
        }
        public HeaderFooter(string footer)
        {
            footerText = footer;
        }

        public override void OnStartPage(PdfWriter writer, Document document)
        {}

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            iTextSharp.text.pdf.draw.LineSeparator line1 = new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.BLACK, Element.ALIGN_BOTTOM, 0);

            Paragraph footer = new Paragraph(footerText, FontFactory.GetFont(FontFactory.TIMES, 12, iTextSharp.text.Font.BOLD));

            footer.Alignment = Element.ALIGN_CENTER;

            PdfPTable footerTbl = new PdfPTable(2);

            footerTbl.TotalWidth = document.PageSize.Width;

            footerTbl.HorizontalAlignment = Element.ALIGN_CENTER;


            PdfPCell cell = new PdfPCell(footer);

            cell.Border = 1;
            cell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            cell.PaddingRight = 50;

            Paragraph pageNumber = new Paragraph(document.PageNumber.ToString(), FontFactory.GetFont(FontFactory.TIMES, 12, iTextSharp.text.Font.BOLD));
            PdfPCell cell1 = new PdfPCell(pageNumber);
            cell1.Border = 1;
            cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            cell1.PaddingLeft = 50;

            footerTbl.AddCell(cell1);
            footerTbl.AddCell(cell);

            footerTbl.WriteSelectedRows(0, 1, 0, 30, writer.DirectContent);


        }



    }
}