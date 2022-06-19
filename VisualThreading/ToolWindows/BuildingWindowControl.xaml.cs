using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisualThreading.ToolWindows.SharedComponents;
using SelectionChangedEventArgs = Community.VisualStudio.Toolkit.SelectionChangedEventArgs;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Media;

namespace VisualThreading.ToolWindows
{
    public partial class BuildingWindowControl : UserControl
    {

        private int id;
        public object draggedItem { get; set; }
        public object draggedItemType { get; set; }
        public Point itemRelativePosition { get; set; }

        private Point preCanvasPos { get; set; }

        private List<List<Block>> InsertedCode { get; set; }


        private readonly Schema.Schema _commands;
        private string _currentLanguage; // file extension for language


        Label preview_effect = new Label()
        {
            Width = Double.NaN,
            Height = Double.NaN,
            Opacity = 0.5
        };

        private int id;

        public System.Windows.Point itemRelativePosition { get;  set; }

        public BuildingWindowControl(Schema.Schema commands, string fileExt)
        {
            _commands = commands;
            _currentLanguage = fileExt;
            InitializeComponent();
            VS.Events.SelectionEvents.SelectionChanged += SelectionEventsOnSelectionChanged; // extends the selection event
            
            this.draggedItem = null;
            this.draggedItemType = null;
            this.InsertedCode = null;
            this.id = 0;
            this.InsertedCode = new List<List<Block>>();
        }

        private void SelectionEventsOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var fileExt = "";
            if (e.To != null)
            {
                var buffer = e.To.Name;
                fileExt =
                    Path.GetExtension(buffer);
            }
            _currentLanguage = fileExt;
        }


        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
            e.Handled = true;
        }

        private void canvasLabels_DragEnter(object sender, DragEventArgs e)
        {
            String dragType = (String)e.Data.GetData("type");
            var dragBackground = e.Data.GetData("background");

            if (dragType != null && dragBackground != null)
            {
                preview_effect.Content = e.Data.GetData("text");
                preview_effect.Background = (Brush)e.Data.GetData("background");
                Point dropPoint = e.GetPosition(this.canvasLabels);

                if (canvasLabels.Children.Contains(preview_effect))
                {
                    preview_effect.Visibility = Visibility.Visible;
                    Canvas.SetLeft(preview_effect, dropPoint.X);
                    Canvas.SetTop(preview_effect, dropPoint.Y);
                } else
                {
                    canvasLabels.Children.Remove(preview_effect);
                    canvasLabels.Children.Add(preview_effect);
                    Canvas.SetLeft(preview_effect, dropPoint.X);
                    Canvas.SetTop(preview_effect, dropPoint.Y);
                }

                System.Diagnostics.Debug.WriteLine(dropPoint);
            }

            e.Handled = true;

        }


        private void Canvas_Drop(object sender, DragEventArgs e)
        {

            System.Diagnostics.Debug.WriteLine("drop_on_canvas");

            String dragType = (String)e.Data.GetData("type");
            var dragBackground = e.Data.GetData("background");
            String dragText = (String)e.Data.GetData("text");
            object draggedItem = e.Data.GetData("draggedItem");
            object itemRelativePosition = e.Data.GetData("itemRelativePosition");


            if (draggedItem != null)
            {
                // new a TextBlock
                TextBlock newTextBlock = new TextBlock();
                newTextBlock.Margin = new Thickness(0, 0, 0, 0);
                newTextBlock.Height = Double.NaN;
                newTextBlock.Width = Double.NaN;
                newTextBlock.FontSize = 18;
                newTextBlock.MouseLeftButtonDown += Label_MouseLeftButtonDown;


                // add the TextBlock to canvas
                var newPos = e.GetPosition(canvasLabels) - this.itemRelativePosition;  // X' Y'
                Point mousePos = e.GetPosition(canvasLabels);  // mouse position

                addBlockToCanvas(sender, dragType, newTextBlock, dragBackground, (Point)newPos, mousePos);

            }

            e.Handled = true;
        }

       

        private void addBlockToCanvas(object sender, String dragType, TextBlock newTextBlock, Object dragBackground, Point pos, Point mousePos)
        {
            if(dragType == "Assignment")
            {
                newTextBlock.Name = "Assignment" + this.id.ToString();
                this.id += 1;

                Run r1 = new Run()
                {
                    Name = "block" + this.id.ToString(),
                    Text = " = ",
                    Background = (Brush)dragBackground
                };
                this.id += 1;
                RegisterName(r1.Name, r1);
                newTextBlock.Inlines.Add(r1);

                bool flag = false;
                // if it is in the blank block position
                for (int i = 0; i < this.InsertedCode.Count; i++)
                {
                    var line = this.InsertedCode[i];
                    for (int j = 0; j < line.Count; j++)
                    {
                        var block = line[j];

                        if (CheckMousePositionInBlock(mousePos, block) == true && block.isDropped == false)
                        {
                            System.Diagnostics.Debug.WriteLine("在block内");
                            flag = true;
                            // set value
                            block.value = r1.Text;
                            block.type = "Assignment";
                            block.isDropped = true;


                            // set shape in canvas
                            Run changedBlock = (Run)((Canvas)sender).FindName(block.name);
                            TextBlock parent = (TextBlock)changedBlock.Parent;
                            changedBlock.Text = r1.Text;
                            changedBlock.Background = (Brush)dragBackground;

                            // add a new block
                            Run r = new Run();
                            r.Text = "          ";
                            r.Background = Brushes.Beige;
                            var addedConditionName = "block" + this.id.ToString();
                            r.Name = addedConditionName;
                            this.id += 1;
                            RegisterName(r.Name, r);
                            parent.Inlines.InsertAfter(changedBlock, r);

                            Block b = new Block()
                            {
                                name = r.Name,
                                value = r.Text,
                                position = (block.position.Item1 + 30, block.position.Item2),
                                width = 80,
                                height = 20,
                                isDropped = false
                            };
                            line.Insert(j + 1, b);
                            for (int k = j + 2; k < line.Count; k++)
                            {
                                var item = line[k];
                                var item1 = item.position.Item1 + 30;
                                item.position = (item1, item.position.Item2);
                            }
                        }
                    }
                }

                if (flag == false)
                {
                    ((Canvas)sender).Children.Add(newTextBlock);
                    Canvas.SetLeft(newTextBlock, pos.X);
                    Canvas.SetTop(newTextBlock, pos.Y);
                }

                System.Diagnostics.Debug.WriteLine(this.InsertedCode);

            }

            if (dragType == "Naming")
            {
                newTextBlock.Name = "Naming" + this.id.ToString();
                this.id += 1;

                Run r1 = new Run()
                {
                    Name = "block" + this.id.ToString(),
                    Text = "nameABC",
                    Background = (Brush)dragBackground
                };
                this.id += 1;
                RegisterName(r1.Name, r1);
                newTextBlock.Inlines.Add(r1);

                bool flag = false;
                // if it is in the blank block position
                for (int i = 0; i < this.InsertedCode.Count; i++)
                {
                    var line = this.InsertedCode[i];
                    for (int j = 0; j < line.Count; j++)
                    {
                        var block = line[j];

                        if (CheckMousePositionInBlock(mousePos, block) == true && block.isDropped == false)
                        {
                            System.Diagnostics.Debug.WriteLine("在block内");
                            flag = true;
                            // set value
                            block.value = r1.Text;
                            block.type = "Naming";
                            block.isDropped = true;


                            // set shape in canvas
                            Run changedBlock = (Run)((Canvas)sender).FindName(block.name);
                            TextBlock parent = (TextBlock)changedBlock.Parent;
                            changedBlock.Text = r1.Text;
                            changedBlock.Background = (Brush)dragBackground;

                            // add a new block
                            Run r = new Run();
                            r.Text = "          ";
                            r.Background = Brushes.Beige;
                            var addedConditionName = "block" + this.id.ToString();
                            r.Name = addedConditionName;
                            this.id += 1;
                            RegisterName(r.Name, r);
                            parent.Inlines.InsertAfter(changedBlock, r);

                            Block b = new Block()
                            {
                                name = r.Name,
                                value = r.Text,
                                position = (block.position.Item1 + 60, block.position.Item2),
                                width = 80,
                                height = 20,
                                isDropped = false
                            };
                            line.Insert(j + 1, b);
                            for (int k = j + 2; k < line.Count; k++)
                            {
                                var item = line[k];
                                var item1 = item.position.Item1 + 60;
                                item.position = (item1, item.position.Item2);
                            }
                        }
                    }
                }

                if (flag == false)
                {
                    ((Canvas)sender).Children.Add(newTextBlock);
                    Canvas.SetLeft(newTextBlock, pos.X);
                    Canvas.SetTop(newTextBlock, pos.Y);
                }

                System.Diagnostics.Debug.WriteLine(this.InsertedCode);
            }

            if (dragType == "MethodPublic")
            {
                // set a name/id
                newTextBlock.Name = "MethodPublic" + this.id.ToString();
                this.id += 1;
                newTextBlock.Tag = true;

                Run r1 = new Run()
                {
                    Name = "block" + this.id.ToString(),
                    Text = " def ",
                    Background = (Brush)dragBackground
                };
                this.id += 1;
                RegisterName(r1.Name, r1);
                newTextBlock.Inlines.Add(r1);

                Block b1 = new Block()
                {
                    name = r1.Name,
                    value = r1.Text,
                    position = (pos.X, pos.Y),
                    width = 40,
                    height = 20,
                    isDropped = true
                };

                Run r2 = new Run()
                {
                    Name = "block" + this.id.ToString(),
                    Text = "             ",
                    Background = Brushes.Beige
                };
                this.id += 1;
                RegisterName(r2.Name, r2);
                newTextBlock.Inlines.Add(r2);

                Block b2 = new Block()
                {
                    name = r2.Name,
                    value = r2.Text,
                    position = (pos.X + 40, pos.Y),
                    width = 80,
                    height = 20,
                    isDropped = false
                };

                Run r3 = new Run()
                {
                    Name = "block" + this.id.ToString(),
                    Text = " ( ",
                    Background = (Brush)dragBackground
                };
                this.id += 1;
                RegisterName(r3.Name, r3);
                newTextBlock.Inlines.Add(r3);

                Block b3 = new Block()
                {
                    name = r3.Name,
                    value = r3.Text,
                    position = (pos.X + 120, pos.Y),
                    width = 20,
                    height = 20,
                    isDropped = true
                };

                Run r4 = new Run()
                {
                    Name = "block" + this.id.ToString(),
                    Text = "              ",
                    Background = Brushes.Beige
                };
                this.id += 1;
                RegisterName(r4.Name, r4);
                newTextBlock.Inlines.Add(r4);

                Block b4 = new Block()
                {
                    name = r4.Name,
                    value = r4.Text,
                    position = (pos.X + 140, pos.Y),
                    width = 80,
                    height = 20,
                    isDropped = false
                };

                Run r5 = new Run()
                {
                    Name = "block" + this.id.ToString(),
                    Text = " ) : \n",
                    Background = (Brush)dragBackground
                };
                this.id += 1;
                RegisterName(r5.Name, r5);
                newTextBlock.Inlines.Add(r5);

                Block b5 = new Block()
                {
                    name = r5.Name,
                    value = r5.Text,
                    position = (pos.X + 220, pos.Y),
                    width = 30,
                    height = 20,
                    isDropped = true
                };

                Run r6 = new Run()
                {
                    Name = "block" + this.id.ToString(),
                    Text = "\t",
                    Background = (Brush)dragBackground
                };
                this.id += 1;
                RegisterName(r6.Name, r6);
                newTextBlock.Inlines.Add(r6);

                Block b6 = new Block()
                {
                    name = r6.Name,
                    value = r6.Text,
                    position = (pos.X, pos.Y + 25),
                    width = 80,
                    height = 20,
                    isDropped = true
                };

                Run r7 = new Run()
                {
                    Name = "block" + this.id.ToString(),
                    Text = "              ",
                    Background = Brushes.Beige
                };
                this.id += 1;
                RegisterName(r7.Name, r7);
                newTextBlock.Inlines.Add(r7);

                Block b7 = new Block()
                {
                    name = r7.Name,
                    value = r7.Text,
                    position = (pos.X + 80, pos.Y + 25),
                    width = 80,
                    height = 20,
                    isDropped = false
                };

                List<Block> line1 = new List<Block>();
                line1.Add(b1);
                line1.Add(b2);
                line1.Add(b3);
                line1.Add(b4);
                line1.Add(b5);
                List<Block> line2 = new List<Block>();
                line2.Add(b6);
                line2.Add(b7);
                this.InsertedCode.Add(line1);
                this.InsertedCode.Add(line2);

                RegisterName(newTextBlock.Name, newTextBlock);
                ((Canvas)sender).Children.Add(newTextBlock);
                Canvas.SetLeft(newTextBlock, pos.X);
                Canvas.SetTop(newTextBlock, pos.Y);

            }

            if (dragType == "VariableInteger")
            {
                newTextBlock.Name = "VariableInteger" + this.id.ToString();
                this.id += 1;

                Run r1 = new Run()
                {
                    Name = "block" + this.id.ToString(),
                    Text = "Int a",
                    Background = (Brush)dragBackground
                };
                this.id += 1;
                RegisterName(r1.Name, r1);
                newTextBlock.Inlines.Add(r1);

                bool flag = false;
                // if it is in the blank block position
                for (int i = 0; i < this.InsertedCode.Count; i++)
                {
                    var line = this.InsertedCode[i];
                    for (int j = 0; j < line.Count; j++)
                    {
                        var block = line[j];
                        System.Diagnostics.Debug.WriteLine(block.position);
                        System.Diagnostics.Debug.WriteLine(block.width);
                        System.Diagnostics.Debug.WriteLine(block.height);
                        if (CheckMousePositionInBlock(mousePos, block) == true && block.isDropped == false)
                        {
                            System.Diagnostics.Debug.WriteLine("在block内");
                            flag = true;
                            // set value
                            block.value = r1.Text;
                            block.type = "VariableInteger";
                            block.isDropped = true;


                            // set shape in canvas
                            Run changedBlock = (Run)((Canvas)sender).FindName(block.name);
                            TextBlock parent = (TextBlock)changedBlock.Parent;
                            changedBlock.Text = r1.Text;
                            changedBlock.Background = (Brush)dragBackground;

                            // add a new block
                            Run r = new Run();
                            r.Text = "          ";
                            r.Background = Brushes.Beige;
                            var addedConditionName = "block" + this.id.ToString();
                            r.Name = addedConditionName;
                            this.id += 1;
                            RegisterName(r.Name, r);
                            parent.Inlines.InsertAfter(changedBlock, r);

                            Block b = new Block()
                            {
                                name = r.Name,
                                value = r.Text,
                                position = (block.position.Item1 + 40, block.position.Item2),
                                width = 80,
                                height = 20,
                                isDropped = false
                            };
                            line.Insert(j + 1, b);
                            for (int k = j+2; k < line.Count; k++)
                            {
                                var item = line[k];
                                var item1 = item.position.Item1 + 40;
                                item.position = (item1, item.position.Item2);
                            }

                            
                        }
                    }
                }

                if (flag == false)
                {
                    ((Canvas)sender).Children.Add(newTextBlock);
                    Canvas.SetLeft(newTextBlock, pos.X);
                    Canvas.SetTop(newTextBlock, pos.Y);
                }

                System.Diagnostics.Debug.WriteLine(this.InsertedCode);
            }

            if (dragType == "OperatorAnd")
            {
                newTextBlock.Name = "OperatorAnd" + this.id.ToString();
                this.id += 1;

                Run r1 = new Run()
                {
                    Name = "block" + this.id.ToString(),
                    Text = "AND",
                    Background = (Brush)dragBackground
                };
                this.id += 1;
                RegisterName(r1.Name, r1);
                newTextBlock.Inlines.Add(r1);

                bool flag = false;
                // if it is in the blank block position
                for (int i = 0; i < this.InsertedCode.Count; i++)
                {
                    var line = this.InsertedCode[i];
                    for (int j = 0; j < line.Count; j++)
                    {
                        var block = line[j];
                        System.Diagnostics.Debug.WriteLine(block.position);
                        System.Diagnostics.Debug.WriteLine(block.width);
                        System.Diagnostics.Debug.WriteLine(block.height);
                        if (CheckMousePositionInBlock(mousePos, block) == true && block.isDropped == false)
                        {
                            System.Diagnostics.Debug.WriteLine("在block内");
                            flag = true;
                            // set value
                            block.value = r1.Text;
                            block.type = "OperatorAnd";
                            block.isDropped = true;


                            // set shape in canvas
                            Run changedBlock = (Run)((Canvas)sender).FindName(block.name);
                            TextBlock parent = (TextBlock)changedBlock.Parent;
                            changedBlock.Text = r1.Text;
                            changedBlock.Background = (Brush)dragBackground;

                            // add a new block
                            Run r = new Run();
                            r.Text = "          ";
                            r.Background = Brushes.Beige;
                            var addedConditionName = "block" + this.id.ToString();
                            r.Name = addedConditionName;
                            this.id += 1;
                            RegisterName(r.Name, r);
                            parent.Inlines.InsertAfter(changedBlock, r);

                            Block b = new Block()
                            {
                                name = r.Name,
                                value = r.Text,
                                position = (block.position.Item1 + 30, block.position.Item2),
                                width = 80,
                                height = 20,
                                isDropped = false
                            };
                            line.Insert(j + 1, b);
                            for (int k = j + 2; k < line.Count; k++)
                            {
                                var item = line[k];
                                var item1 = item.position.Item1 + 80;
                                item.position = (item1, item.position.Item2);
                            }
                        }
                    }
                }

                if (flag == false)
                {
                    ((Canvas)sender).Children.Add(newTextBlock);
                    Canvas.SetLeft(newTextBlock, pos.X);
                    Canvas.SetTop(newTextBlock, pos.Y);
                }

                System.Diagnostics.Debug.WriteLine(this.InsertedCode);
            }
            canvasLabels.Children.Remove(preview_effect);
            this.draggedItem = null;
            if (dragType == "ComparatorEqual")
            {

            }

            if (dragType == "ConditionIf")
            {

            }

            if (dragType == "LoopWhile")
            {

            }

        }


        private bool CheckMousePositionInBlock(Point mousePosition, Block block)
        {
            var mouse_x = mousePosition.X;
            var mouse_y = mousePosition.Y;

            var block_pos = block.position;
            var block_width = block.width;
            var block_height = block.height;

            if (mouse_x >= block_pos.Item1 && mouse_x <= (block_pos.Item1 + block_width) && mouse_y >= block_pos.Item2 && mouse_y <= (block_pos.Item2 + block_height))
            {
                return true;
            }

            return false;

        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("label MouseLeftButtonDown");

            this.draggedItem = (TextBlock)sender;
            this.itemRelativePosition = e.GetPosition((TextBlock)this.draggedItem);  // 鼠标相对于指定元素的相对位置
            this.preCanvasPos = e.GetPosition(canvasLabels);

            e.Handled = true;
        }


        private void CanvasLabel_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Canvas PreviewMouseMove");

            if (this.draggedItem == null)
            {
                return;
            } else
            {
                var newPos = e.GetPosition(canvasLabels) - this.itemRelativePosition;  // X Y => X' Y'
                System.Diagnostics.Debug.WriteLine(newPos);

                int height_range = (int)(canvasLabels.ActualHeight / 5);
                int width_range = (int)(canvasLabels.ActualWidth / 5);

                if (newPos.Y < -50 || newPos.Y + height_range > canvasLabels.ActualHeight || newPos.X < -width_range || newPos.X + width_range > canvasLabels.ActualWidth)
                {
                    UIElement textBlock = this.draggedItem as UIElement;
                    textBlock.Visibility = Visibility.Collapsed;

                }
                else
                {
                    UIElement textBlock = this.draggedItem as UIElement;
                    textBlock.Visibility = Visibility.Visible;
                    Canvas.SetTop((UIElement)this.draggedItem, newPos.Y);
                    Canvas.SetLeft((UIElement)this.draggedItem, newPos.X);
                }
            }

            canvasLabels.CaptureMouse();
            e.Handled = true;
        }

        

        private void CanvasLabel_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Canvas PreviewMouseLeftButtonUp");

            if (this.draggedItem != null)
            {

                UIElement uiElement = this.draggedItem as UIElement;
                if (uiElement.Visibility != Visibility.Visible)
                {
                    canvasLabels.Children.Remove(uiElement);
                }

                // 松开label拖动的一瞬间
                System.Diagnostics.Debug.WriteLine(this.draggedItem.GetType());

                // change all blocks' position for one element
                List<String> l = new List<string>();
                foreach (var item in cur.Inlines)
                {
                    l.Add(item.Name);
                }

                for (int i = 0; i < this.InsertedCode.Count; i++)
                {
                    var line = this.InsertedCode[i];
                    for (int j = 0; j < line.Count; j++)
                    {
                        var block = line[j];
                        if (l.Contains(block.name))
                        {

                            var pos = block.position;
                            var changedPos = (pos.Item1 + changedValue.X, pos.Item2 + changedValue.Y);
                            block.position = changedPos;
                        }

                    }
                }
            }
            
            
        }

        private void CanvasLabel_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Canvas PreviewMouseLeftButtonUp");

            if (this.draggedItem != null)
            {

                this.draggedItem = null;
                canvasLabels.ReleaseMouseCapture();

            }
            canvasLabels.Children.Remove(preview_effect);
            e.Handled = true;

        }

        private void canvasLabels_DragLeave(object sender, DragEventArgs e)
        {
            preview_effect.Visibility = Visibility.Collapsed;
            e.Handled = true;
        }
    }

    class CodeValue
    {
        public List<SpecialStatement> statements { get; set; }
        public List<OperateVariable> variables { get; set; }
        public List<OperateVariable> operators { get; set; }
        public string value { get; set; }

        public CodeValue()
        {
            this.statements = new List<SpecialStatement>();
            this.variables = new List<OperateVariable>();
            this.operators = new List<OperateVariable>();
        }

    }


    class Block
    {
        public String name { get; set; }
        public string value { get; set; }
        public string type { get; set; }

        public (double, double) position { get; set; }
        public int width { get; set; }
        public int height { get; set; }

        public bool isDropped { get; set; }

    }

    
}
