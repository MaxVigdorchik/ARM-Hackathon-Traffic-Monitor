   M S B A M L   `   `   `  ����X  TARM-Hackathon-Traffic-Monitor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM+clr-namespace:ARM_Hackathon_Traffic_MonitorARM_Hackathon_Traffic_Monitor  5   	   .    (ARM_Hackathon_Traffic_Monitor.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   =local+clr-namespace:ARM_Hackathon_Traffic_Monitor     5   	     � Title$  
MainWindow��5   	   $	��800��6   $
��1400��6)   
�������.��5	      � .+�5
   
   �� -   5        $	��Map��5
      $
��Left=�6   $	��700��67   $��	50,33,0,0q�6D   $	��Top=�6W   $
��1000��6o   �� 5   
   $��Right=�6   $	��750��6-   $��1050,10,0,0q�6:   $	��Top=�6O   $	��300��6g   ���	#FF00A8FF.+�5      �� $��Center=�6   $��Center=�66   $	��280��6Q    �� Left$ 10��6^   
 �� Top$ 10��6o   .+�5      �� -   5         ��Name$ 	nodeLabel��5      $��NodeID:��6+   $��20(�6=   �� 5      -   5         ��Name$ 	NodeIDBox��5      $��20(�6-    �� TextWrapping$
 Wrap=�6;   $��Hover over Node to Select��6O   �� 5      -   5        $ 	edgeLabel��5      $ ��Edge from node {A} to {B}:��6+   $��20(�6P   �� 5      -   5        $ EdgeBox��5      $��20(�6+   $
 Wrap=�69   $��Hover over Edge to Select��6M   �� 5      -   5        $ weightLabel��5      $��Weighting of Edge:��6-   $��20(�6J   �� 5      -   5        $ 	WeightBox��5      $��20(�6-   $
 Wrap=�6;   $��Estimated traversal time��6O   �� 5      -   5        $ stats��5      $��Most recent packet��6'   $��20(�6D   �� 5      -	   5        $ 	PacketBox��5      $	��450��6-   $
 Wrap=�6:   $�� ��6N   �� 5      -
   5         ��Name$ 
QuitButton��5      $
��Quit��6-   6<   �� 5      -   5        $ RestartButton��5      $��Restart��60   6B   5      5      5      5      