   M S B A M L   `   `   `  ���� @  <VIPER, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:VIPER.PresentersVIPER.Presenters  clr-namespace:VIPERVIPER  3clr-namespace:VIPER.PropertiesVIPER.Properties      VIPER.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5
presentersclr-namespace:VIPER.Presenters     %viperclr-namespace:VIPER     .resclr-namespace:VIPER.Properties     -     �Name$  _this��    VIPER.Properties.Resources   MainWindowTitle � Title# Z $	��650��$
��1200�� � 	Resources  	  Count��%   &	        S� ����  �� 
TargetType" �.���  �� Property � �� �� Value ��  �� 
TargetType" �.R�]� 
�������.+��  � RowDefinitions �� $��* ��� $	��190 �.+�   VIPER.Presenters.Subsystem  	  Name$	 	subsystem��$��0��$
��Auto=�$
��Auto=�� $��1��
������� �� $��45 ��� $��45 ��� $��70 �
 � ColumnDefinitions
 �� $��30 ��� $	��260 ��� $	��120 ��� $	��150 ��� $	��180 ��� $	��150 ��� $��* �.+�	�  	�Name$	 RB1��-   $��2��$��0��  AddModuleButton_Icon#��[   AddElementButtonStyle#��[ 	� $	 RB2��-   $��2��$��1��  AddNodeButton_Icon#��[ #��[ 	� $	 RB3��-   $��2��$��2��  AddLineButton_Icon#��[ #��[ �� $��3��$��0�� �� Orientation$ Vertical=�.+���    ElementCount#��Z $��5,5,5,1q��� -   $��3��$��0��$��	15,0,15,0q�.@���  �� 
IsSelected . .��1�� .��2�� .��3�� .��4�� .��5�� .��6�� .��7�� .��8�� .��9�� .��10�� .��15�� $��3��$��1��$ Vertical=�.+��� $��0q�   ElementPosition#��Z $��5,5,5,1q��� -   $��	15,0,15,0q�.@���  .    
Horizontal#��Z ��    Vertical#��Z �� $��3��$��2��$ Vertical=�$��	0,0,0,-17q�.+���  ��Name$ Clear��-      	ClearForm#��Z $��	15,2,15,0q��� -      DeleteSelect#��Z $��	15,2,15,0q��� $
 Save��-	      SaveAs#��Z $��	15,2,15,0q��� $
 Open��-
      Open#��Z $��	15,2,15,0q��� $��4��$��0��$ Vertical=�.+��� $ GetGraph��-      	GetRating#��Z $��	15,2,15,0q���  ��Name$ Repair��-    d� 	IsChecked$
 trueY�   	UseRepair#��Z $��	15,2,15,0q��� $��4��$��1��$ Vertical=�.+��� $ 	ShowGraph��-   $
 trueY�   	ShowGraph#��Z $��	15,2,15,0q��� $ CalculateEquations��-   $
 trueY�   CalculateEquations#��Z $��	15,2,15,0q��� $��4��$��2��$ Vertical=�.+��� $ 
ShowCharts��-   $
 trueY�   
ShowCharts#��Z $��	15,2,15,0q�� $��10,10,10,10q�
 �� $
��Auto ��� $��* � �� $
��Auto ��� $
��Auto �.+��� $��0��$��0��   Step#��Z �� $
��Step��$��0��$��10,0,5,5q�$��1��$��10��$	��Top=��� $��1��$��0��
   Max#��Z �� $��MaxTime��$��1��$��10,0,5,5q�$��1��$	��210���� $��	listView1��$��Disabled=�$��1��$��3��$	��140��$��10,10,10,10q�$	��Top=�����   VIPER.Presenters.Path    LogFormElements! �� Source#! Z  " �� View" �� .P��� # �� Width$	# 250��$   	LogHeader#��Z$ % �� DisplayMemberBinding% �� �� & �� CornerRadius$& 10��$��5��$��3��$��10,10,10,10q�
�������$��2q����� " ��' �� Triggers' �� ( �� Binding( �� ) �� ElementName$) 	subsystem��* �� Path$* SelectedModule�+ �� Value+ Z� .a���  � y�$ Hidden=�._�� $��3q�, � DataContext, ��   VIPER.Presenters.Module-   SelectedModule#* Z- 
 �� $��* ��� $��* � �� $��* ��� $��* ��� $��* ��� $��* ��� $��* �.+��� $��Center=�$��2��$��0��.+��� . �� Padding$. 0,5,0,5q�$
��NAME���� $. 0,5,0,0q�$��0��$��1��$��λ���� $. 0,5,0,0q�$��0��$��2��$��μ���� $. 0,5,0,0q�$��0��$��3��$��repairs���� $��1��$��1������ $) 	subsystem��$* SelectedModule.Lambda�/ �� Mode$/ TwoWay=�0 �� UpdateSourceTrigger$0 	LostFocus=��� $��1��$��2������ $) 	subsystem��$* SelectedModule.Mu�$/ TwoWay=��� $��1��$��3������ $) 	subsystem��$ * SelectedModule.RepairCount�$/ TwoWay=��� $��6��$��5��$ Vertical=�.+��� $ Remove��-   $��10,10,10,10q�1   GetCondition#��Z1 �� 2 ��Name$2 Formula��$��Disabled=�$
��Auto=�3 �� TextWrapping$
3 Wrap=�4 �� AcceptsReturn4 . $��90��$��10,10,10,10q�$
��Auto��$
��16px(�
���   �
�������