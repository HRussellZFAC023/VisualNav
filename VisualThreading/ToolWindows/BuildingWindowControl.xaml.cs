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

        public object draggedItem { get; set; }
        public object draggedItemType { get; set; }
        internal CodeValue codevalue { get; set; }

        private readonly Schema.Schema _commands;
        private string _currentLanguage; // file extension for language

        private object DraggedItem { get; set; }

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
            DraggedItem = null;
            VS.Events.SelectionEvents.SelectionChanged += SelectionEventsOnSelectionChanged; // extends the selection event
            this.draggedItemType = null;
            this.codevalue = new CodeValue();
            this.id = 0;
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


        /*private void Label_MouseMove_From_List(object sender, MouseEventArgs e)
        {
            Label label = sender as Label;

            if (label != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var type = label.Name;
                var runObject = (Run)label.Content;
                var bgColor = runObject.Background;
                var text = runObject.Text;
                DataObject data = new DataObject();
                data.SetData("type", type);
                data.SetData("background", bgColor);
                data.SetData("text", text);

                this.draggedItem = (UIElement)sender;
                this.itemRelativePosition = e.GetPosition((IInputElement)this.draggedItem);

                DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);

            }
        }*/

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

            System.Diagnostics.Debug.WriteLine("canvas_drop");

            String dragType = (String)e.Data.GetData("type");
            var dragBackground = e.Data.GetData("background");
            String text = (String)e.Data.GetData("text");
            object draggedItem = e.Data.GetData("draggedItem");
            object itemRelativePosition = e.Data.GetData("itemRelativePosition");

            this.draggedItem = (UIElement)draggedItem;
            this.itemRelativePosition = (Point)itemRelativePosition;

            if (dragType != null && dragBackground != null)
            {
                TextBlock newLabel = new TextBlock();
                newLabel.Margin = new Thickness(0, 0, 0, 0);
                newLabel.Height = Double.NaN;
                newLabel.Width = Double.NaN;
                newLabel.FontSize = 18;
                newLabel.MouseLeftButtonDown += Label_MouseLeftButtonDown;


                // if statement
                if (dragType == "IfStatementLabel") 
                {
                    System.Diagnostics.Debug.WriteLine("if statement");

                    // 设置name
                    newLabel.Name = "if" + this.id.ToString();
                    this.id += 1;

                    // 设置 shape
                    newLabel.Inlines.Add(new Run()
                    {
                        Text = "if( ",
                        Background = (Brush)dragBackground
                    });

                    // 添加一个block condition
                    Run r = new Run();
                    r.Text = "              ";
                    r.Background = Brushes.Beige;
                    var conditionName = "block" + this.id.ToString();
                    r.Name = conditionName;
                    RegisterName(r.Name, r);
                    newLabel.Inlines.Add(r);
                    this.id += 1;

                    newLabel.Inlines.Add(new Run()
                    {
                        Text = " ) {\n",
                        Background = (Brush)dragBackground
                    });
                    newLabel.Inlines.Add(new Run()
                    {
                        Text = "      ",
                        Background = (Brush)dragBackground,

                    });

                    // 添加一个block statement
                    Run r2 = new Run();
                    r2.Text = "              ";
                    r2.Background = Brushes.Beige;
                    var statementName = "block" + this.id.ToString();
                    r2.Name = statementName;
                    RegisterName(r2.Name, r2);
                    newLabel.Inlines.Add(r2);
                    this.id += 1;

                    newLabel.Inlines.Add(new Run()
                    {
                        Text = "\n}\n",
                        Background = (Brush)dragBackground
                    });

                    // 把added element shape添加进canvas
                    RegisterName(newLabel.Name, newLabel);
                    ((Canvas)sender).Children.Add(newLabel);
                    var pos = e.GetPosition(canvasLabels) - this.itemRelativePosition;  // X' Y'
                    Canvas.SetLeft(newLabel, pos.X);
                    Canvas.SetTop(newLabel, pos.Y);


                    // 把 element value 添加进 codeValue
                    OperateVariable condition = new OperateVariable()
                    {
                        name = conditionName,
                        position = (pos.X + 25, pos.Y),
                        width = 60,
                        height = 20
                    };
                    
                    OperateVariable s = new OperateVariable()
                    {
                        name = statementName,
                        position = (pos.X + 40, pos.Y + 25),
                        width = 60,
                        height = 20
                    };
                    List<OperateVariable> s1 = new List<OperateVariable>();
                    s1.Add(s);

                    IfStatement oneIf = new IfStatement();
                    oneIf.type = "if";
                    oneIf.name = newLabel.Name;
                    oneIf.addCondition(condition);
                    oneIf.addStatement(s1);

                    this.codevalue.statements.Add(oneIf);

                    System.Diagnostics.Debug.WriteLine("add if statement successfully");
                }


                // if else statement
                if (dragType == "IfelseStatementLabel")
                {
                    System.Diagnostics.Debug.WriteLine("if else statement");

                    // 设置shape

                    // 设置name
                    newLabel.Name = "ifelse" + this.id.ToString();
                    this.id += 1;

                    newLabel.Inlines.Add(new Run()
                    {
                        Text = "if( ",
                        Background = (Brush)dragBackground

                    });

                    // 添加一个block condition
                    Run r1 = new Run();
                    r1.Text = "              ";
                    r1.Background = Brushes.Beige;
                    var conditionName = "block" + this.id.ToString();
                    r1.Name = conditionName;
                    RegisterName(r1.Name, r1);
                    newLabel.Inlines.Add(r1);
                    this.id += 1;

                    newLabel.Inlines.Add(new Run()
                    {
                        Text = " ) {\n",
                        Background = (Brush)dragBackground

                    });
                    newLabel.Inlines.Add(new Run()
                    {
                        Text = "        ",
                        Background = (Brush)dragBackground

                    });

                    // 添加第一个block statement
                    Run r2 = new Run();
                    r2.Text = "              ";
                    r2.Background = Brushes.Beige;
                    var statement1Name = "block" + this.id.ToString();
                    r2.Name = statement1Name;
                    RegisterName(r2.Name, r2);
                    newLabel.Inlines.Add(r2);
                    this.id += 1;

                    newLabel.Inlines.Add(new Run()
                    {
                        Text = "} else {\n",
                        Background = (Brush)dragBackground

                    });
                    newLabel.Inlines.Add(new Run()
                    {
                        Text = "      ",
                        Background = (Brush)dragBackground

                    });

                    // 添加第二个block statement
                    Run r3 = new Run();
                    r3.Text = "              ";
                    r3.Background = Brushes.Beige;
                    var statement2Name = "block" + this.id.ToString();
                    r3.Name = statement2Name;
                    RegisterName(r3.Name, r3);
                    newLabel.Inlines.Add(r3);
                    this.id += 1;

                    
                    newLabel.Inlines.Add(new Run()
                    {
                        Text = "}\n",
                        Background = (Brush)dragBackground

                    });

                    // 把新增 element 添加进 canvas
                    RegisterName(newLabel.Name, newLabel);
                    ((Canvas)sender).Children.Add(newLabel);
                    var pos = e.GetPosition(canvasLabels) - this.itemRelativePosition;  // X' Y'
                    Canvas.SetLeft(newLabel, pos.X);
                    Canvas.SetTop(newLabel, pos.Y);

                    // 把element添加到codeValue
                    OperateVariable c1 = new OperateVariable()
                    {
                        name = conditionName,
                        position = (pos.X + 25, pos.Y),
                        width = 60,
                        height = 20
                    };
                    
                    OperateVariable s1 = new OperateVariable()
                    {
                        name = statement1Name,
                        position = (pos.X + 40, pos.Y + 25),
                        width = 60,
                        height = 20
                    };
                    OperateVariable s2 = new OperateVariable()
                    {
                        name = statement2Name,
                        position = (pos.X + 40, pos.Y + 75),
                        width = 60,
                        height = 20
                    };
                    List<OperateVariable> statement1 = new List<OperateVariable>();
                    statement1.Add(s1);
                    List<OperateVariable> statement2 = new List<OperateVariable>();
                    statement2.Add(s2);

                    IfElseStatement s = new IfElseStatement();
                    s.type = "ifelse";
                    s.name = newLabel.Name;
                    s.addCondition(c1);
                    s.addStatements1(statement1);
                    s.addStatements2(statement2);

                    this.codevalue.statements.Add(s);

                    System.Diagnostics.Debug.WriteLine("add if else statement successfully");
                }


                // variable
                if (dragType == "VariableLabel")
                {

                    System.Diagnostics.Debug.WriteLine("variable");

                    newLabel.FontSize = 14;
                    var flag = false;

                    // 添加 element shape
                    newLabel.Name = "variable" + this.id.ToString();
                    this.id += 1;

                    // add an element shape
                    var t = "Variable1";
                    newLabel.Inlines.Add(new Run()
                    {
                        Text = t,
                        Background = (Brush)dragBackground
                    });

                    // 判断是否有block可以放置
                    // 判断是否鼠标的位置在block的position上
                    var newPos = e.GetPosition(canvasLabels) - this.itemRelativePosition;  // X' Y'
                    var mousePos = e.GetPosition(canvasLabels);  // 鼠标位置
                    System.Diagnostics.Debug.WriteLine((mousePos.X, mousePos.Y));


                    if (this.codevalue.statements != null)
                    {
                        System.Diagnostics.Debug.WriteLine("有可以放置的block");
                        // 有block，判断鼠标的位置是否在block的position上
                        // 得到所有的block
                        var ss = this.codevalue.statements;
                        foreach (SpecialStatement s in ss)
                        {
                            if (s.type == "if")
                            {
                                var conList = ((IfStatement)s).conditions;
                                var sList = ((IfStatement)s).statements;
                                var wantedName = ((IfStatement)s).name;

                                // condition block
                                for(int i = 0; i < conList.Count; i++)
                                {
                                    OperateVariable block = conList[i];
                                    if (CheckMousePosition(mousePos, block))
                                    {
                                        System.Diagnostics.Debug.WriteLine("在if的condition内");

                                        flag = true;

                                        // 添加shape

                                        // 把label设置position 放置到block上
                                        RegisterName(newLabel.Name, newLabel);
                                        ((Canvas)sender).Children.Add(newLabel);
                                        Canvas.SetLeft(newLabel, block.position.Item1);
                                        Canvas.SetTop(newLabel, block.position.Item2);

                                        // 给该condition添加一个额外的block
                                        var addedConditionName = addNewBlockToCondition(sender, wantedName, block, dragBackground);


                                        // 添加value

                                        // 设置被drop的block的value
                                        block.value = newLabel.Text;
                                        block.type = "variable";
                                        block.draggedLabelName = newLabel.Name;

                                        // 添加variable在vodevalue的值
                                        this.codevalue.variables.Add(new OperateVariable()
                                        {
                                            name = newLabel.Name,
                                            value = newLabel.Text,
                                            type = "variable",
                                            position = (block.position.Item1, block.position.Item2),
                                            width = 60,
                                            height = 20
                                        });

                                        // 把新增的block添加到value里，注意position
                                        OperateVariable c1 = new OperateVariable()
                                        {
                                            name = addedConditionName,
                                            position = (block.position.Item1 + 85, block.position.Item2),
                                            width = 60,
                                            height = 20
                                        };
                                        conList.Add(c1);

                                    }
                                }

                                for(int i = 0; i < sList.Count; i++)
                                {
                                    List<OperateVariable> st = sList[i];

                                    for (int j = 0; j < st.Count; j++)
                                    {
                                        OperateVariable block = st[j];

                                        if (CheckMousePosition(mousePos, block))
                                        {
                                            System.Diagnostics.Debug.WriteLine("在if的statement内");

                                            flag = true;

                                            // 添加shape

                                            // 把label设置position 放置到block上
                                            RegisterName(newLabel.Name, newLabel);
                                            ((Canvas)sender).Children.Add(newLabel);
                                            Canvas.SetLeft(newLabel, block.position.Item1);
                                            Canvas.SetTop(newLabel, block.position.Item2);

                                            // 设置被drop的block值
                                            block.value = newLabel.Text;
                                            block.type = "variable";
                                            block.draggedLabelName = newLabel.Name;

                                            // 添加variable在vodevalue的值
                                            this.codevalue.variables.Add(new OperateVariable()
                                            {
                                                name = newLabel.Name,
                                                value = newLabel.Text,
                                                type = "variable",
                                                position = (block.position.Item1, block.position.Item2),
                                                width = 60,
                                                height = 20
                                            });

                                            // 给该statement添加两个额外的block
                                            if (j == 0)
                                            {
                                                var (addedStatementName1, addedStatementName2) = addNewBlockToStatement(sender, wantedName, block, dragBackground);
                                                OperateVariable c1 = new OperateVariable()
                                                {
                                                    name = addedStatementName1,
                                                    position = (block.position.Item1 + 85, block.position.Item2),
                                                    width = 60,
                                                    height = 20
                                                };
                                                st.Add(c1);
                                                OperateVariable s1 = new OperateVariable()
                                                {
                                                    name = addedStatementName2,
                                                    position = (block.position.Item1, block.position.Item2 + 25),
                                                    width = 60,
                                                    height = 20
                                                };
                                                List<OperateVariable> newStatement = new List<OperateVariable>();
                                                newStatement.Add(s1);
                                                sList.Add(newStatement);

                                            } else
                                            {
                                                var addedStatementName = addNewBlockToCondition(sender, wantedName, block, dragBackground);
                                                OperateVariable c1 = new OperateVariable()
                                                {
                                                    name = addedStatementName,
                                                    position = (block.position.Item1 + 85, block.position.Item2),
                                                    width = 60,
                                                    height = 20
                                                };
                                                st.Add(c1);
                                            }
                                            
   
                                        }
                                    }
                                }


                            } else if(s.type == "ifelse")
                            {
                                var conList = ((IfElseStatement)s).conditions;
                                var s1List = ((IfElseStatement)s).statements1;
                                var s2List = ((IfElseStatement)s).statements2;
                                var wantedName = ((IfElseStatement)s).name;

                                for (int i = 0; i < conList.Count; i++)
                                {
                                    OperateVariable block = conList[i];

                                    if (CheckMousePosition(mousePos, block))
                                    {
                                        System.Diagnostics.Debug.WriteLine("在ifelse的condition内");

                                        flag = true;

                                        // 添加shape

                                        // 把label设置position 放置到block上
                                        RegisterName(newLabel.Name, newLabel);
                                        ((Canvas)sender).Children.Add(newLabel);
                                        Canvas.SetLeft(newLabel, block.position.Item1);
                                        Canvas.SetTop(newLabel, block.position.Item2);

                                        // 给该condition添加一个额外的block
                                        var addedConditionName = addNewBlockToCondition(sender, wantedName, block, dragBackground);


                                        // 添加value

                                        // 设置被drop的block值
                                        block.value = newLabel.Text;
                                        block.type = "variable";
                                        block.draggedLabelName = newLabel.Name;

                                        // 添加variable在vodevalue的值
                                        this.codevalue.variables.Add(new OperateVariable()
                                        {
                                            name = newLabel.Name,
                                            value = newLabel.Text,
                                            type = "variable",
                                            position = (block.position.Item1, block.position.Item2),
                                            width = 60,
                                            height = 20
                                        });

                                        // 把新增的block添加到value里，注意position
                                        OperateVariable c1 = new OperateVariable()
                                        {
                                            name = addedConditionName,
                                            position = (block.position.Item1 + 85, block.position.Item2),
                                            width = 60,
                                            height = 20
                                        };
                                        conList.Add(c1);

                                        
                                    }
                                }

                                for (int i = 0; i < s1List.Count; i++)
                                {
                                    List<OperateVariable> st = s1List[i];
                                    for (int j = 0; j < st.Count; j++)
                                    {
                                        OperateVariable block = st[j];

                                        if (CheckMousePosition(mousePos, block))
                                        {
                                            System.Diagnostics.Debug.WriteLine("在ifelse的statement1内");

                                            flag = true;

                                            // 添加shape

                                            // 把label设置position 放置到block上
                                            RegisterName(newLabel.Name, newLabel);
                                            ((Canvas)sender).Children.Add(newLabel);
                                            Canvas.SetLeft(newLabel, block.position.Item1);
                                            Canvas.SetTop(newLabel, block.position.Item2);

                                            // 设置被drop的block值
                                            block.value = newLabel.Text;
                                            block.type = "variable";
                                            block.draggedLabelName = newLabel.Name;

                                            // 添加variable在vodevalue的值
                                            this.codevalue.variables.Add(new OperateVariable()
                                            {
                                                name = newLabel.Name,
                                                value = newLabel.Text,
                                                type = "variable",
                                                position = (block.position.Item1, block.position.Item2),
                                                width = 60,
                                                height = 20
                                            });

                                            // 给该statement添加两个额外的block
                                            if (j == 0)
                                            {
                                                var (addedStatementName1, addedStatementName2) = addNewBlockToStatement(sender, wantedName, block, dragBackground);
                                                OperateVariable c1 = new OperateVariable()
                                                {
                                                    name = addedStatementName1,
                                                    position = (block.position.Item1 + 85, block.position.Item2),
                                                    width = 60,
                                                    height = 20
                                                };
                                                st.Add(c1);
                                                OperateVariable s1 = new OperateVariable()
                                                {
                                                    name = addedStatementName2,
                                                    position = (block.position.Item1, block.position.Item2 + 25),
                                                    width = 60,
                                                    height = 20
                                                };
                                                List<OperateVariable> newStatement = new List<OperateVariable>();
                                                newStatement.Add(s1);
                                                s1List.Add(newStatement);

                                            }
                                            else
                                            {
                                                var addedStatementName = addNewBlockToCondition(sender, wantedName, block, dragBackground);
                                                OperateVariable c1 = new OperateVariable()
                                                {
                                                    name = addedStatementName,
                                                    position = (block.position.Item1 + 85, block.position.Item2),
                                                    width = 60,
                                                    height = 20
                                                };
                                                st.Add(c1);
                                            }


                                        }
                                    }
                                }

                                for (int i = 0; i < s2List.Count; i++)
                                {
                                    List<OperateVariable> st = s2List[i];
                                    for (int j = 0; j < st.Count; j++)
                                    {
                                        OperateVariable block = st[j];

                                        if (CheckMousePosition(mousePos, block))
                                        {
                                            System.Diagnostics.Debug.WriteLine("在ifelse的statement2内");

                                            flag = true;

                                            // 添加shape

                                            // 把label设置position 放置到block上
                                            RegisterName(newLabel.Name, newLabel);
                                            ((Canvas)sender).Children.Add(newLabel);
                                            Canvas.SetLeft(newLabel, block.position.Item1);
                                            Canvas.SetTop(newLabel, block.position.Item2);

                                            // 设置被drop的block值
                                            block.value = newLabel.Text;
                                            block.type = "variable";
                                            block.draggedLabelName = newLabel.Name;

                                            // 添加variable在vodevalue的值
                                            this.codevalue.variables.Add(new OperateVariable()
                                            {
                                                name = newLabel.Name,
                                                value = newLabel.Text,
                                                type = "variable",
                                                position = (block.position.Item1, block.position.Item2),
                                                width = 60,
                                                height = 20
                                            });

                                            // 给该statement添加两个额外的block
                                            if (j == 0)
                                            {
                                                var (addedStatementName1, addedStatementName2) = addNewBlockToStatement(sender, wantedName, block, dragBackground);
                                                OperateVariable c1 = new OperateVariable()
                                                {
                                                    name = addedStatementName1,
                                                    position = (block.position.Item1 + 85, block.position.Item2),
                                                    width = 60,
                                                    height = 20
                                                };
                                                st.Add(c1);
                                                OperateVariable s1 = new OperateVariable()
                                                {
                                                    name = addedStatementName2,
                                                    position = (block.position.Item1, block.position.Item2 + 25),
                                                    width = 60,
                                                    height = 20
                                                };
                                                List<OperateVariable> newStatement = new List<OperateVariable>();
                                                newStatement.Add(s1);
                                                s2List.Add(newStatement);

                                            }
                                            else
                                            {
                                                var addedStatementName = addNewBlockToCondition(sender, wantedName, block, dragBackground);
                                                OperateVariable c1 = new OperateVariable()
                                                {
                                                    name = addedStatementName,
                                                    position = (block.position.Item1 + 85, block.position.Item2),
                                                    width = 60,
                                                    height = 20
                                                };
                                                st.Add(c1);
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("没有可以放置的block");
                        // 没有可以放置的block，直接添加并放置
                        // 添加到canvas
                        ((Canvas)sender).Children.Add(newLabel);
                        Canvas.SetLeft(newLabel, newPos.X);
                        Canvas.SetTop(newLabel, newPos.Y);

                        // 添加到codeValue
                        this.codevalue.variables.Add(new OperateVariable()
                        {
                            name = newLabel.Name,
                            value = newLabel.Text,
                            type = "variable",
                            position = (newPos.X + 85, newPos.Y + 25),
                            width = 60,
                            height = 20
                        });
                    }

                    if(flag == false)
                    {
                        System.Diagnostics.Debug.WriteLine("不在block内");

                        // 添加到canvas
                        ((Canvas)sender).Children.Add(newLabel);
                        Canvas.SetLeft(newLabel, newPos.X);
                        Canvas.SetTop(newLabel, newPos.Y);

                        // 添加到codeValue
                        this.codevalue.variables.Add(new OperateVariable()
                        {
                            name = newLabel.Name,
                            value = newLabel.Text,
                            type = "variable",
                            position = (newPos.X + 85, newPos.Y + 20),
                            width = 60,
                            height = 20
                        });
                    }

                }

                // operator
                if (dragType == "OperatorLabel")
                {

                    System.Diagnostics.Debug.WriteLine("operator");

                    newLabel.FontSize = 14;
                    var flag = false;

                    // 添加 element shape
                    newLabel.Name = "operator" + this.id.ToString();
                    this.id += 1;

                    // add an element shape
                    var t = "Operator1";
                    newLabel.Inlines.Add(new Run()
                    {
                        Text = t,
                        Background = (Brush)dragBackground
                    });

                    // 判断是否有block可以放置
                    // 判断是否鼠标的位置在block的position上
                    var newPos = e.GetPosition(canvasLabels) - this.itemRelativePosition;  // X' Y'
                    var mousePos = e.GetPosition(canvasLabels);  // 鼠标位置
                    System.Diagnostics.Debug.WriteLine((mousePos.X, mousePos.Y));


                    if (this.codevalue.statements != null)
                    {
                        System.Diagnostics.Debug.WriteLine("有可以放置的block");
                        // 有block，判断鼠标的位置是否在block的position上
                        // 得到所有的block
                        var ss = this.codevalue.statements;
                        foreach (SpecialStatement s in ss)
                        {
                            if (s.type == "if")
                            {
                                var conList = ((IfStatement)s).conditions;
                                var sList = ((IfStatement)s).statements;
                                var wantedName = ((IfStatement)s).name;

                                // condition block
                                for (int i = 0; i < conList.Count; i++)
                                {
                                    OperateVariable block = conList[i];
                                    if (CheckMousePosition(mousePos, block))
                                    {
                                        System.Diagnostics.Debug.WriteLine("在if的condition内");

                                        flag = true;

                                        // 添加shape

                                        // 把label设置position 放置到block上
                                        RegisterName(newLabel.Name, newLabel);
                                        ((Canvas)sender).Children.Add(newLabel);
                                        Canvas.SetLeft(newLabel, block.position.Item1);
                                        Canvas.SetTop(newLabel, block.position.Item2);

                                        // 给该condition添加一个额外的block
                                        var addedConditionName = addNewBlockToCondition(sender, wantedName, block, dragBackground);


                                        // 添加value

                                        // 设置被drop的block的value
                                        block.value = newLabel.Text;
                                        block.type = "operator";
                                        block.draggedLabelName = newLabel.Name;

                                        // 添加variable在vodevalue的值
                                        this.codevalue.variables.Add(new OperateVariable()
                                        {
                                            name = newLabel.Name,
                                            value = newLabel.Text,
                                            type = "operator",
                                            position = (block.position.Item1, block.position.Item2),
                                            width = 60,
                                            height = 20
                                        });

                                        // 把新增的block添加到value里，注意position
                                        OperateVariable c1 = new OperateVariable()
                                        {
                                            name = addedConditionName,
                                            position = (block.position.Item1 + 25, block.position.Item2),
                                            width = 60,
                                            height = 20
                                        };
                                        conList.Add(c1);

                                    }
                                }

                                for (int i = 0; i < sList.Count; i++)
                                {
                                    List<OperateVariable> st = sList[i];
                                    for (int j = 0; j < st.Count; j++)
                                    {
                                        OperateVariable block = st[j];

                                        if (CheckMousePosition(mousePos, block))
                                        {
                                            System.Diagnostics.Debug.WriteLine("在if的statement内");

                                            flag = true;

                                            // 添加shape

                                            // 把label设置position 放置到block上
                                            RegisterName(newLabel.Name, newLabel);
                                            ((Canvas)sender).Children.Add(newLabel);
                                            Canvas.SetLeft(newLabel, block.position.Item1);
                                            Canvas.SetTop(newLabel, block.position.Item2);

                                            // 设置被drop的block值
                                            block.value = newLabel.Text;
                                            block.type = "variable";
                                            block.draggedLabelName = newLabel.Name;

                                            // 添加variable在vodevalue的值
                                            this.codevalue.variables.Add(new OperateVariable()
                                            {
                                                name = newLabel.Name,
                                                value = newLabel.Text,
                                                type = "variable",
                                                position = (block.position.Item1, block.position.Item2),
                                                width = 60,
                                                height = 20
                                            });

                                            // 给该statement添加两个额外的block
                                            if (j == 0)
                                            {
                                                var (addedStatementName1, addedStatementName2) = addNewBlockToStatement(sender, wantedName, block, dragBackground);
                                                OperateVariable c1 = new OperateVariable()
                                                {
                                                    name = addedStatementName1,
                                                    position = (block.position.Item1 + 85, block.position.Item2),
                                                    width = 60,
                                                    height = 20
                                                };
                                                st.Add(c1);
                                                OperateVariable s1 = new OperateVariable()
                                                {
                                                    name = addedStatementName2,
                                                    position = (block.position.Item1, block.position.Item2 + 25),
                                                    width = 60,
                                                    height = 20
                                                };
                                                List<OperateVariable> newStatement = new List<OperateVariable>();
                                                newStatement.Add(s1);
                                                sList.Add(newStatement);

                                            }
                                            else
                                            {
                                                var addedStatementName = addNewBlockToCondition(sender, wantedName, block, dragBackground);
                                                OperateVariable c1 = new OperateVariable()
                                                {
                                                    name = addedStatementName,
                                                    position = (block.position.Item1 + 85, block.position.Item2),
                                                    width = 60,
                                                    height = 20
                                                };
                                                st.Add(c1);
                                            }

                                        }
                                    }
                                }

                            }
                            else if (s.type == "ifelse")
                            {
                                var conList = ((IfElseStatement)s).conditions;
                                var s1List = ((IfElseStatement)s).statements1;
                                var s2List = ((IfElseStatement)s).statements2;
                                var wantedName = ((IfElseStatement)s).name;

                                for (int i = 0; i < conList.Count; i++)
                                {
                                    OperateVariable block = conList[i];

                                    if (CheckMousePosition(mousePos, block))
                                    {
                                        System.Diagnostics.Debug.WriteLine("在ifelse的condition内");

                                        flag = true;

                                        // 添加shape

                                        // 把label设置position 放置到block上
                                        RegisterName(newLabel.Name, newLabel);
                                        ((Canvas)sender).Children.Add(newLabel);
                                        Canvas.SetLeft(newLabel, block.position.Item1);
                                        Canvas.SetTop(newLabel, block.position.Item2);

                                        // 给该condition添加一个额外的block
                                        var addedConditionName = addNewBlockToCondition(sender, wantedName, block, dragBackground);


                                        // 添加value

                                        // 设置被drop的block值
                                        block.value = newLabel.Text;
                                        block.type = "operator";
                                        block.draggedLabelName = newLabel.Name;

                                        // 添加variable在vodevalue的值
                                        this.codevalue.variables.Add(new OperateVariable()
                                        {
                                            name = newLabel.Name,
                                            value = newLabel.Text,
                                            type = "operator",
                                            position = (block.position.Item1, block.position.Item2),
                                            width = 60,
                                            height = 20
                                        });

                                        // 把新增的block添加到value里，注意position
                                        OperateVariable c1 = new OperateVariable()
                                        {
                                            name = addedConditionName,
                                            position = (block.position.Item1 + 25, block.position.Item2),
                                            width = 60,
                                            height = 20
                                        };
                                        conList.Add(c1);


                                    }
                                }

                                for (int i = 0; i < s1List.Count; i++)
                                {
                                    List<OperateVariable> st = s1List[i];
                                    for (int j = 0; j < st.Count; j++)
                                    {
                                        OperateVariable block = st[j];

                                        if (CheckMousePosition(mousePos, block))
                                        {
                                            System.Diagnostics.Debug.WriteLine("在ifelse的statement1内");

                                            flag = true;

                                            // 添加shape

                                            // 把label设置position 放置到block上
                                            RegisterName(newLabel.Name, newLabel);
                                            ((Canvas)sender).Children.Add(newLabel);
                                            Canvas.SetLeft(newLabel, block.position.Item1);
                                            Canvas.SetTop(newLabel, block.position.Item2);

                                            // 设置被drop的block值
                                            block.value = newLabel.Text;
                                            block.type = "variable";
                                            block.draggedLabelName = newLabel.Name;

                                            // 添加variable在vodevalue的值
                                            this.codevalue.variables.Add(new OperateVariable()
                                            {
                                                name = newLabel.Name,
                                                value = newLabel.Text,
                                                type = "variable",
                                                position = (block.position.Item1, block.position.Item2),
                                                width = 60,
                                                height = 20
                                            });

                                            // 给该statement添加两个额外的block
                                            if (j == 0)
                                            {
                                                var (addedStatementName1, addedStatementName2) = addNewBlockToStatement(sender, wantedName, block, dragBackground);
                                                OperateVariable c1 = new OperateVariable()
                                                {
                                                    name = addedStatementName1,
                                                    position = (block.position.Item1 + 85, block.position.Item2),
                                                    width = 60,
                                                    height = 20
                                                };
                                                st.Add(c1);
                                                OperateVariable s1 = new OperateVariable()
                                                {
                                                    name = addedStatementName2,
                                                    position = (block.position.Item1, block.position.Item2 + 25),
                                                    width = 60,
                                                    height = 20
                                                };
                                                List<OperateVariable> newStatement = new List<OperateVariable>();
                                                newStatement.Add(s1);
                                                s1List.Add(newStatement);

                                            }
                                            else
                                            {
                                                var addedStatementName = addNewBlockToCondition(sender, wantedName, block, dragBackground);
                                                OperateVariable c1 = new OperateVariable()
                                                {
                                                    name = addedStatementName,
                                                    position = (block.position.Item1 + 85, block.position.Item2),
                                                    width = 60,
                                                    height = 20
                                                };
                                                st.Add(c1);
                                            }
                                        }  
                                    }
                                }

                                for (int i = 0; i < s2List.Count; i++)
                                {
                                    List<OperateVariable> st = s2List[i];
                                    for (int j = 0; j < st.Count; j++)
                                    {
                                        OperateVariable block = st[j];

                                        if (CheckMousePosition(mousePos, block))
                                        {
                                            System.Diagnostics.Debug.WriteLine("在ifelse的statement2内");

                                            flag = true;

                                            // 添加shape

                                            // 把label设置position 放置到block上
                                            RegisterName(newLabel.Name, newLabel);
                                            ((Canvas)sender).Children.Add(newLabel);
                                            Canvas.SetLeft(newLabel, block.position.Item1);
                                            Canvas.SetTop(newLabel, block.position.Item2);

                                            // 设置被drop的block值
                                            block.value = newLabel.Text;
                                            block.type = "variable";
                                            block.draggedLabelName = newLabel.Name;

                                            // 添加variable在vodevalue的值
                                            this.codevalue.variables.Add(new OperateVariable()
                                            {
                                                name = newLabel.Name,
                                                value = newLabel.Text,
                                                type = "variable",
                                                position = (block.position.Item1, block.position.Item2),
                                                width = 60,
                                                height = 20
                                            });

                                            // 给该statement添加两个额外的block
                                            if (j == 0)
                                            {
                                                var (addedStatementName1, addedStatementName2) = addNewBlockToStatement(sender, wantedName, block, dragBackground);
                                                OperateVariable c1 = new OperateVariable()
                                                {
                                                    name = addedStatementName1,
                                                    position = (block.position.Item1 + 85, block.position.Item2),
                                                    width = 60,
                                                    height = 20
                                                };
                                                st.Add(c1);
                                                OperateVariable s1 = new OperateVariable()
                                                {
                                                    name = addedStatementName2,
                                                    position = (block.position.Item1, block.position.Item2 + 25),
                                                    width = 60,
                                                    height = 20
                                                };
                                                List<OperateVariable> newStatement = new List<OperateVariable>();
                                                newStatement.Add(s1);
                                                s2List.Add(newStatement);

                                            }
                                            else
                                            {
                                                var addedStatementName = addNewBlockToCondition(sender, wantedName, block, dragBackground);
                                                OperateVariable c1 = new OperateVariable()
                                                {
                                                    name = addedStatementName,
                                                    position = (block.position.Item1 + 85, block.position.Item2),
                                                    width = 60,
                                                    height = 20
                                                };
                                                st.Add(c1);
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("没有可以放置的block");
                        // 没有可以放置的block，直接添加并放置
                        // 添加到canvas
                        RegisterName(newLabel.Name, newLabel);
                        ((Canvas)sender).Children.Add(newLabel);
                        Canvas.SetLeft(newLabel, newPos.X);
                        Canvas.SetTop(newLabel, newPos.Y);

                        // 添加到codeValue
                        this.codevalue.variables.Add(new OperateVariable()
                        {
                            name = newLabel.Name,
                            value = newLabel.Text,
                            type = "operator",
                            position = (newPos.X, newPos.Y),
                            width = 60,
                            height = 20
                        });
                    }

                    if (flag == false)
                    {
                        System.Diagnostics.Debug.WriteLine("不在block内");

                        // 添加到canvas
                        RegisterName(newLabel.Name, newLabel);
                        ((Canvas)sender).Children.Add(newLabel);
                        Canvas.SetLeft(newLabel, newPos.X);
                        Canvas.SetTop(newLabel, newPos.Y);

                        // 添加到codeValue
                        this.codevalue.variables.Add(new OperateVariable()
                        {
                            name = newLabel.Name,
                            value = newLabel.Text,
                            type = "operator",
                            position = (newPos.X, newPos.Y),
                            width = 60,
                            height = 20
                        });
                    }

                }

            }
            canvasLabels.Children.Remove(preview_effect);
            this.draggedItem = null;
            e.Handled = true;
        }

  

        private String addNewBlockToCondition(object sender, string wantedName, OperateVariable block, Object dragBackground)
        {
            System.Diagnostics.Debug.WriteLine(wantedName);
            TextBlock wantedNode = (TextBlock)((Canvas)sender).FindName(wantedName);

            System.Diagnostics.Debug.WriteLine(block.name);
            Run wantedBlock = (Run)wantedNode.FindName(block.name);

            Run r = new Run();
            r.Text = "              ";
            r.Background = Brushes.Beige;
            var conditionName = "block" + this.id.ToString();
            r.Name = conditionName;
            this.id += 1;
            RegisterName(r.Name, r);
            wantedNode.Inlines.InsertAfter(wantedBlock, r);
            return conditionName;

        }

        private (String, String) addNewBlockToStatement(object sender, string wantedName, OperateVariable block, Object dragBackground)
        {
            TextBlock wantedNode = (TextBlock)((Canvas)sender).FindName(wantedName);
            Run wantedBlock = (Run)wantedNode.FindName(block.name);

            Run r = new Run();
            r.Text = "              ";
            r.Background = Brushes.Beige;
            var statementName = "block" + this.id.ToString();
            r.Name = statementName;
            this.id += 1;
            RegisterName(r.Name, r);
            wantedNode.Inlines.InsertAfter(wantedBlock, r);

            Run r3 = new Run()
            {
                Text = "\n      ",
                Background = wantedNode.Background,

            };
            wantedNode.Inlines.InsertAfter(r, r3);

            Run r2 = new Run();
            r2.Text = "              ";
            r2.Background = Brushes.Beige;
            var statementName2 = "block" + this.id.ToString();
            r2.Name = statementName2;
            this.id += 1;
            RegisterName(r2.Name, r2);
            wantedNode.Inlines.InsertAfter(r3, r2);

            return (statementName, statementName2);

        }


        private bool CheckMousePosition(Point mousePosition, OperateVariable block)
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

            this.draggedItem = (UIElement)sender;
            this.itemRelativePosition = e.GetPosition((IInputElement)this.draggedItem);  // 鼠标相对于指定元素的相对位置
            e.Handled = true;
        }


        private void CanvasLabel_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            /*System.Diagnostics.Debug.WriteLine("Canvas PreviewMouseMove");*/

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

                if(this.draggedItem.GetType().Equals(typeof(TextBlock)))
                {
                    System.Diagnostics.Debug.WriteLine("11111111111111");
                    var item = (TextBlock)this.draggedItem;
                    var type = item.Text;
                    System.Diagnostics.Debug.WriteLine(type);
                }

                // 只有variable和operator被拖动的时候
                // if / if-else statement 拖动以后改block的位置


                bool flag = false;
                // 在drop的一瞬间，获取到鼠标的位置
                var newPos = e.GetPosition(canvasLabels) - this.itemRelativePosition;  // X Y => X' Y'
                var mousePosition = newPos;
                System.Diagnostics.Debug.WriteLine(mousePosition);

                // 判断鼠标位置离哪个框最近



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

    class SpecialStatement
    {
        public String type { get; set; }
        public int id { get; set; }
    }

    class IfStatement : SpecialStatement
    {
        public String name { get; set; }
        public List<OperateVariable> conditions { get; set; }
        public List<List<OperateVariable>> statements { get; set; }
        public IfStatement()
        {
            this.conditions = new List<OperateVariable>();
            this.statements = new List<List<OperateVariable>>();
        }
        public void addCondition(OperateVariable v)
        {
            this.conditions.Add(v);
        }
        public void addStatement(List<OperateVariable> s)
        {
            this.statements.Add(s);
        }
        public List<OperateVariable> getConditions()
        {
            return this.conditions;
        }
    }

    class IfElseStatement: SpecialStatement
    {
        public String name { get; set; }
        public List<OperateVariable> conditions { get; set; }
        public List<List<OperateVariable>> statements1 { get; set; }
        public List<List<OperateVariable>> statements2 { get; set; }

        public IfElseStatement()
        {
            this.conditions = new List<OperateVariable>();
            this.statements1 = new List<List<OperateVariable>>();
            this.statements2 = new List<List<OperateVariable>>();
        }

        public void addCondition(OperateVariable v)
        {
            this.conditions.Add(v);
        }
        public void addStatements1(List<OperateVariable> s)
        {
            this.statements1.Add(s);
        }
        public void addStatements2(List<OperateVariable> s)
        {
            this.statements2.Add(s);
        }

    }


    class OperateVariable
    {
        public String name { get; set; }
        public string value { get; set; }
        public string type { get; set; }
        public (double, double) position { get; set; }
        public int width { get; set; }
        public int height { get; set; }

        public String draggedLabelName { get; set; }

    }


}
