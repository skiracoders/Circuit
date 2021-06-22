using Silk.NET.Maths;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System;

namespace Skira
{
    public class ActivatableButton : Button
    {
        protected ImageAttributes activeImageAttributes, inactiveImageAttributes;
        protected bool active;
        public bool Active
        {
            get => active;
            set
            {
                if (value)
                {
                    if (!active)
                    {
                        //Console.WriteLine("Activating");
                        active = true;
                        ImageAttributes = activeImageAttributes;
                    }
                }
                else
                {
                    if (active)
                    {
                        //Console.WriteLine("Deactivating");
                        active = false;
                        ImageAttributes = inactiveImageAttributes;
                    }
                }
            }
        }
        public ActivatableButton(ImagePack imagePack, Point origin, Size size, ImageAttributes activeImageAttributes, ImageAttributes inactiveImageAttributes) : base(imagePack, origin, size,
            inactiveImageAttributes)
        {
            this.inactiveImageAttributes = inactiveImageAttributes;
            this.activeImageAttributes = activeImageAttributes;
            Active = false;

        }
    }
    public class DeletePlaceButton : ActivatableButton
    {
        public Cell Target;
        public DeletePlaceButton(ImagePack imagePack, Point origin, Size size, ImageAttributes activeImageAttributes,
            ImageAttributes inactiveImageAttributes) : base(imagePack, origin, size, activeImageAttributes, inactiveImageAttributes)
        {
            Target = null;
        }
    }
    public class SelectionButton : Button
    {
        public Type Type;
        public SelectionButton(ImagePack imagePack, Point origin, Size size, ImageAttributes imageAttributes, Type type) : base(imagePack, origin, size,
            imageAttributes)
        {
            Type = type;
        }
    }
    public class DirectionButton : Button
    {
        public Direction Direction;
        public DirectionButton(ImagePack imagePack, Point origin, Size size, ImageAttributes imageAttributes, Direction direction) : base(imagePack, origin, size,
            imageAttributes)
        {
            Direction = direction;
        }
    }
    public class GameInterface
    {
        private Vector2D<int> size;
        private LinkedList<SelectionButton> selectionButtons;
        private LinkedList<DirectionButton> directionButtons;
        private DeletePlaceButton placeButton;
        private DeletePlaceButton deleteButton;

        private SelectionButton selectionButton;
        private DirectionButton directionButton;
        private ImageManager imageManager;
        public GameInterface()
        {
            size = GameManager.Instance.Size;
            selectionButtons = new LinkedList<SelectionButton>();
            directionButtons = new LinkedList<DirectionButton>();
            imageManager = GameManager.Instance.ImageManager;
            int gap = 4;
            Size buttonSize = new Size(16, 16);
            directionButton = AddDirection(imageManager["Arrow-Up"], new Point(gap, size.Y - (gap + buttonSize.Height) * 1), buttonSize,
                GameManager.Instance.DarkAttributes, Direction.Up);
            AddDirection(imageManager["Arrow-Right"], new Point(gap, size.Y - (gap + buttonSize.Height) * 2), buttonSize,
                GameManager.Instance.DarkAttributes, Direction.Right);
            AddDirection(imageManager["Arrow-Down"], new Point(gap, size.Y - (gap + buttonSize.Height) * 3), buttonSize,
                GameManager.Instance.DarkAttributes, Direction.Down);
            AddDirection(imageManager["Arrow-Left"], new Point(gap, size.Y - (gap + buttonSize.Height) * 4), buttonSize,
                GameManager.Instance.DarkAttributes, Direction.Left);

            placeButton = new DeletePlaceButton(imageManager["Place"], new Point(gap, size.Y - (gap + buttonSize.Height) * 5), buttonSize,
                GameManager.Instance.BrightAttributes, GameManager.Instance.DarkAttributes);
            deleteButton = new DeletePlaceButton(imageManager["Delete"], new Point(gap, size.Y - (gap + buttonSize.Height) * 6), buttonSize,
                GameManager.Instance.BrightAttributes, GameManager.Instance.DarkAttributes);


            selectionButton = AddSelection(imageManager["Positive"], new Point(size.X - gap - buttonSize.Width, size.Y - (gap + buttonSize.Height) * 1),
                buttonSize, GameManager.Instance.DarkAttributes, typeof(PositiveSource));
            AddSelection(imageManager["Negative"], new Point(size.X - gap - buttonSize.Width, size.Y - (gap + buttonSize.Height) * 2), buttonSize,
                GameManager.Instance.DarkAttributes, typeof(NegativeSource));
            //AddButton(GameManager.Instance.ImageManager["Arrow-Up"], new Vector2D<int>(), new Vector2D<int>());
            //AddButton(GameManager.Instance.ImageManager["Arrow-Up"], new Vector2D<int>(), new Vector2D<int>());
            //AddButton(GameManager.Instance.ImageManager["Arrow-Up"], new Vector2D<int>(), new Vector2D<int>());
            selectionButton.ImageAttributes = GameManager.Instance.BrightAttributes;
            directionButton.ImageAttributes = GameManager.Instance.BrightAttributes;
        }
        public SelectionButton AddSelection(ImagePack imagePack, Point origin, Size size, ImageAttributes imageAttributes, Type type)
        {
            SelectionButton selectionButton = new SelectionButton(imagePack, origin, size, imageAttributes, type);
            selectionButtons.AddLast(selectionButton);
            return selectionButton;
        }
        public DirectionButton AddDirection(ImagePack imagePack, Point origin, Size size, ImageAttributes imageAttributes, Direction direction)
        {
            DirectionButton directionButton = new DirectionButton(imagePack, origin, size, imageAttributes, direction);
            directionButtons.AddLast(directionButton);
            return directionButton;
        }
        public void Draw(Graphics graphics)
        {
            placeButton.Draw(graphics);
            deleteButton.Draw(graphics);
            foreach (SelectionButton selectionButton in selectionButtons)
            {
                selectionButton.Draw(graphics);
            }
            foreach (DirectionButton directionButton in directionButtons)
            {
                directionButton.Draw(graphics);
            }
        }
        public void Revalidate(Cell cell)
        {
            Cell target = GameManager.Instance.World.GetNeighbour(cell, directionButton.Direction);
            placeButton.Target = target;
            deleteButton.Target = target;
            if (target == null)
            {
                placeButton.Active = false;
                deleteButton.Active = false;
                return;
            }
            if (target.Occupant == null)
            {
                placeButton.Active = true;
                deleteButton.Active = false;
                return;
            }
            placeButton.Active = false;
            deleteButton.Active = true;
        }
        public void LeftMouseDown(Point point)
        {
            if (placeButton.Bounds.Contains(point))
            {
                if (placeButton.Active)
                {
                    placeButton.Target.Place(selectionButton.Type);
                    Revalidate(GameManager.Instance.Player.Cell);
                }
                return;
            }
            if (deleteButton.Bounds.Contains(point))
            {
                if (deleteButton.Active)
                {
                    deleteButton.Target.Remove();
                    Revalidate(GameManager.Instance.Player.Cell);
                }
                return;
            }
            foreach (SelectionButton selectionButton in selectionButtons)
            {
                if (selectionButton.Bounds.Contains(point))
                {
                    this.selectionButton.ImageAttributes = GameManager.Instance.DarkAttributes;
                    this.selectionButton = selectionButton;
                    this.selectionButton.ImageAttributes = GameManager.Instance.BrightAttributes;
                    return;
                }
            }
            foreach (DirectionButton directionButton in directionButtons)
            {
                if (directionButton.Bounds.Contains(point))
                {
                    this.directionButton.ImageAttributes = GameManager.Instance.DarkAttributes;
                    this.directionButton = directionButton;
                    this.directionButton.ImageAttributes = GameManager.Instance.BrightAttributes;
                    Revalidate(GameManager.Instance.Player.Cell);
                    return;
                }
            }
        }
    }
}