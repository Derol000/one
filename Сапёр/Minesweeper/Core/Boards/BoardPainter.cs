﻿using System.Collections.Generic;
using System.Drawing;

namespace Minesweeper.Core.Boards
{
    public class BoardPainter
    {
        public Board Board { get; set; }
        public Cell HoveredCell { get; set; }

        private Dictionary<int, SolidBrush> _cellColours;
        private readonly Font _textFont = new Font("Verdana", 16f, FontStyle.Bold);
        private readonly Font _percentFont = new Font(FontFamily.GenericSansSerif, 7f);
        private readonly Font _locationFont = new Font(FontFamily.GenericSansSerif, 6f);

        public BoardPainter()
        {
            SetupColours();
        }

        /// <summary>
        /// Configures the colours used to render the cell counts.
        /// </summary>
        private void SetupColours()
        {
            if (_cellColours == null)
            {
                _cellColours = new Dictionary<int, SolidBrush> {
                    { 0, new SolidBrush(ColorTranslator.FromHtml("0xffffff")) },
                    { 1, new SolidBrush(ColorTranslator.FromHtml("0x0000FE")) },
                    { 2, new SolidBrush(ColorTranslator.FromHtml("0x186900")) },
                    { 3, new SolidBrush(ColorTranslator.FromHtml("0xAE0107")) },
                    { 4, new SolidBrush(ColorTranslator.FromHtml("0x000177")) },
                    { 5, new SolidBrush(ColorTranslator.FromHtml("0x8D0107")) },
                    { 6, new SolidBrush(ColorTranslator.FromHtml("0x007A7C")) },
                    { 7, new SolidBrush(ColorTranslator.FromHtml("0x902E90")) },
                    { 8, new SolidBrush(ColorTranslator.FromHtml("0x000000")) }
                };
            }
        }

        /// <summary>
        /// Paints the game board.
        /// </summary>
        /// <param name="graphics"></param>
        public void Paint(Graphics graphics)
        {
            graphics.Clear(Color.White);
            graphics.TranslateTransform(Board.CellSize / 2, Board.CellSize * 2);

            for (int x = 0; x < Board.Width; x++)
            {
                for (int y = 0; y < Board.Height; y++)
                {
                    var cell = Board.Cells[x, y];
                    DrawInsideCell(cell, graphics);
                    graphics.DrawRectangle(Pens.DimGray, cell.Bounds);
                }
            }
        }

        /// <summary>
        /// Gets the brush colour to paint the background of the cell with.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private Brush GetBackgroundBrush(Cell cell)
        {
            if (HoveredCell != null && Board.ShowCellHighlights)
            {
                // The hovered cell itself:
                if (HoveredCell.XLoc == cell.XLoc && HoveredCell.YLoc == cell.YLoc)
                {
                    return Brushes.LightSteelBlue;
                }

               
                if (HoveredCell.GetNeighborCells().Contains(cell))
                {
                    return Brushes.LightSkyBlue;
                }

                
            }

           
            if (cell.Opened)
                return Brushes.LightGray;
            else if (cell.MinePercentage == 0M && Board.ShowHints)
                return Brushes.PaleGreen;
            else if (cell.MinePercentage == 100M && Board.ShowHints)
                return Brushes.Salmon;

            return Brushes.DarkGray;
        }

      
        private void DrawInsideCell(Cell cell, Graphics graphics)
        {
            // Closed Cell
            if (cell.Closed)
            {
                graphics.FillRectangle(GetBackgroundBrush(cell), cell.Bounds);

                if (cell.MinePercentage != -1 && Board.ShowPercentage)
                {
                    graphics.DrawString($"{cell.MinePercentage.ToString()}%", _percentFont, Brushes.DarkRed, cell.TopLeftPos);
                }
            }

            
            if (cell.Opened)
            {
                graphics.FillRectangle(GetBackgroundBrush(cell), cell.Bounds);

                if (cell.NumMines > 0)
                {
                    graphics.DrawString(cell.NumMines.ToString(), _textFont, GetCellColour(cell), cell.CenterPos);
                }
            }

            // Flagged Cell
            if (cell.Flagged)
            {
                graphics.DrawString("?", _textFont, Brushes.Black, cell.CenterPos);
            }

            // Mine Cell
            if (cell.IsMine && (Board.ShowMines || Board.GameOver))
            {
                // This cell was the one that ended the game
                if (cell.Opened)
                {
                    graphics.FillRectangle(Brushes.Red, cell.Bounds);
                }

                // Reveal the locations of the mines that had not been flagged
                if (!cell.Flagged)
                {
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    graphics.DrawString("M", _textFont, Brushes.Black, cell.Bounds, format);
                }
            }

            // Draw (x,y) location
            if (Board.ShowLocation)
            {
                graphics.DrawString($"{cell.XLoc},{cell.YLoc}", _locationFont, Brushes.Black, cell.BottomLeftPos);
            }
        }

        /// <summary>
        /// Return the colour code associated with the number of surrounding mines
        /// </summary>
        /// <returns></returns>
        private SolidBrush GetCellColour(Cell cell)
        {
            return _cellColours[cell.NumMines];
        }
    }
}
