# exam_trading_system

Description:
A console based program handling trading of items between users. 

Runtime overview:
All the menus in the program are designed for numbered input choices. 
When starting the program the user is presented with logging in, registering a new account or closing the program.

- Register account; Here the user inputs name, email and password where there is a confirmation check on the password to see if you input correctly. At any stage in the process the user can cancel and go back to menu. 
- Login; Login functions much the same, however only email and password is needed. the user only gets an error message saying it's incorrect after both credentials are entered, as to not know which was wrong for better security. 

After logging in the user gets to the main menu. Here the options are either Trade, Add Item To Market, View Your Items or Log Out.

- Trade; Here are 4 options:
    - Propose Trade, in which you get to enter a number of the user's position in the list. when entered you can choose between the users available items and your own for a trade. 
    - Browse Trade Requests, here are 4 options
        - Sent, Only visual, the user sees it's own sent requests
        - Recieved, here the user gets to approve or deny an incoming request by first choosing request number and then inputting approve or deny. if approve the item's switch owners
        - Completed Requests, here are all requests that are not pending and have been resolved. 
        - Back to menu
    - Show Market items, shows all available items that are not the users own. 
    - Back to Main Menu
- Add Item to Market, functions basically the same as login but instead with items. User inputs name and description
- View your items, displays all your owned items.
_____________________________________________________________________________________________

Code discussion:

I have several classes/enums that weren't explicitly required in the exercise such as TradeSystem, Utility and Menu.
TradeSystem I implemented because I wanted to have an object with persistent tracking of the three lists that was going to be used in the program that could be reached from any where in the program. Something that couldn't be done if the lists were declared in Main. 

My code hasn't needed interfaces nor inheritance since the classes different variables and methods are so different from eachother. With the exception of the method Info() which is in the classes: Item, User and Trade. Even here the method is wildly different depending on which class it's in making inheritence not useful in this case. 
An interface wasn't something I have needed either with the Info() method, if I would need to call several classes Info method at the same time it would be useful but that was never needed in my code. 