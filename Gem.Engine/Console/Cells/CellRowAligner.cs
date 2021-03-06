﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Gem.Infrastructure.Events;
using Microsoft.Xna.Framework;
using Gem.Infrastructure.Functional;
using Gem.Engine.Console.Rendering;

namespace Gem.Engine.Console.Cells
{

    public class CellAlignerEventArgs : EventArgs
    {
        private readonly Row row;
        private readonly int rowIndex;

        public CellAlignerEventArgs(Row row, int rowIndex)
        {
            this.rowIndex = rowIndex;
            this.row = row;
        }

        public int RowIndex { get { return rowIndex; } }
        public Row Row { get { return row; } }
    }

    public class CellAlignmentOptions
    {
        public event EventHandler<EventArgs> OnOptionChanged;

        public CellAlignmentOptions(int cellSpacing)
        {
            this.spacing = cellSpacing;
        }

        private int spacing;
        public int CellSpacing
        {
            get { return spacing; }
            set
            {
                spacing = value;
                OnOptionChanged.RaiseEvent(this, EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// Aligns ICell instances into rows according to the specified buffer axis x-size
    /// </summary>
    public class CellRowAligner
    {

        #region Fields

        private readonly List<Row> rows;
        private readonly Func<char, Vector2> characterMeasurer;
        #endregion

        #region Ctor

        public CellRowAligner(Func<char,Vector2> characterMeasurer)
        {
            this.rows = new List<Row>();
            this.characterMeasurer = characterMeasurer;
        }

        #endregion

        #region Events

        public event EventHandler<CellAlignerEventArgs> RowAdded;
        public event EventHandler<EventArgs> Cleared;

        #endregion

        #region Arrange Rows Algorithm

        public void Reset()
        {
            rows.Clear();
            Cleared.RaiseEvent(this, EventArgs.Empty);
        }

        public void AlignToRows(IEnumerable<ICell> cells, int spacing, float rowSize)
        {
            this.Reset();
            int currentRowSize = 0;
            int skippedEntries = 0;
            int cellsCounter = 0;

            foreach (var cell in cells)
            {
                currentRowSize += ((int)characterMeasurer(cell.Content).X + spacing);
                if (currentRowSize > rowSize)
                {
                    cellsCounter++;
                    var row = new Row(rows.Count, cells.Skip(skippedEntries).Take(cellsCounter - skippedEntries));
                    rows.Add(row);
                    skippedEntries = cellsCounter;
                    currentRowSize = 0;
                    RowAdded.RaiseEvent(this, new CellAlignerEventArgs(row, rows.Count - 1));
                }
                else
                {
                    cellsCounter++;
                }
            }

            //add the remaining cells
            if (cellsCounter - skippedEntries >= 0)
            {
                var row = new Row(rows.Count, cells.Skip(skippedEntries).Take(cellsCounter - skippedEntries));
                rows.Add(row);
                RowAdded.RaiseEvent(this, new CellAlignerEventArgs(row, rows.Count - 1));
            }
        }

        #endregion

        #region Get Rows

        public IEnumerable<Row> Rows()
        {
            return rows;
        }

        public IEnumerable<ICell> Cells()
        {
            List<ICell> cells = new List<ICell>();
            rows.ForEach(row=>cells.AddRange(row.Entries));
            return cells;
        }

        #endregion

    }
}
