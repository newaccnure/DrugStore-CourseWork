   M S B A M L   `   `   `  ����C  ?WPF_TEST, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:WPF_TESTWPF_TEST  5   	       WPF_TEST.BuyProductWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:WPF_TEST     5   	     � Title$#  Просмотр товара��5   	   $	��800��6!   $
��1200��6.    � WindowStartupLocation$ CenterScreen=�6;   .��5
      �� .��5   
   �� .+�5      �  � Orientation$ 
Horizontal=�6   .+�5      �� -   5        $��	DrugImage��5      $	��300��6)   $	��300��65   ��6B   �� �� 5      .+�5      �� -   5        $��DrugNameLabel��5      $��10q�61   $��25(�6=   �� 5      -   5        $��Availability��5      $��10q�60   $��25(�6<   �� 5      -   5        $��
PriceLabel��5      $��10q�6.   $��25(�6:   �� 5      -   5        $��AmountLabel��5      $��Количество��6/   $��10q�6D   $��25(�6P   � 5      -   5        $��	emptyGrid��5       � RowDefinitions 5      �� 5       � ColumnDefinitions 5      �� 5      $	��180 �6/   �� 5      $��* �6/   5      �� 5      -   5        $��WholesalePrices��5   !   $��10q�68   5      �� 5!      -   5        $��Купить��5!   5   $��25(�6F   $��10q�6T   $��20 10 20 10q�6`   5#      5$      �� 5%      $"��Характеристики��6   $��10q�6-   $��25(�69   �� 5&      -	   5        $��ManufacturersChar��5&   <   $��10q�6    �� Orientation$ Vertical=�6%   6<   �� 5'      -
   5        $��CountriesChar��5'   <   $��10q�6   $ Vertical=�6%   6<   �� 5(      $��Описание:��6   $��10q�6(   $��25(�64   ]� 5)      .+�5*      �� -   5        $��DescriptionTextBox��5*   F   $
��Left=�6   $
��Wrap=�62   $��20(�6`   $
��Auto��6n   $��10q�6|   $	��750��6�   �� 5+      $
��Left=�6   5,      �� 5-      $��Применение:��6   $��25(�6*   $��10q�68   ]� 5.      .+�5/      �� -   5        $��ApplicationTextBox��5/   F   $
��Left=�6   $
��Wrap=�62   $��20(�6`   $
��Auto��6n   $��10q�6|   $	��750��6�   �� 50      $
��Left=�6   51      �� 52      $'��!Противопоказания:��6   $��25(�60   $��10q�6>   ]� 53      .+�54      �� -   5        $��WarningTextBox��54   F   $
��Left=�6   $
��Wrap=�62   $��20(�6\   $
��Auto��6j   $��10q�6x   $	��750��6�   �� 55      $
��Left=�6   56      57      58      59      