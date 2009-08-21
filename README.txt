Light Weight Event System .Net Binding (LWES.net)

COPYRIGHT (C) 2009, Phillip Clark (cerebralkungfu[at*g mail[dot*com)
  original .NET implementation
 
LWES.net is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

LWES.net is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with LWES.net.  If not, see <http://www.gnu.org/licenses/>.

__________________
2009-08-13 Version 0.1.* ALPHA - Happy Birthday to me: LWES DotNet Binding.
	
	This is the initial check-in - it remains a work in progress. I've been working
	with this code for a couple of weeks, un-protected by version control, so it 
	was time to pull the trigger.
	
	The project requires Visual Studio .NET 2008 and .NET 3.0 (or above). 
	Be warned: I HAVE NOT tested all possible configurations! Also note that
	your network and firewall settings may prevent some of the tests from
	succeeding. 
	
	I noticed during development that when I was debugging from a computer that
	hosts VMWare the virtual device driver prevented the mutlicast traffic from
	being sent. I was debugging the java TestListener on Ubuntu Linux and couldn't
	"hear" the multicast events from my Vista64 box until I uninstalled VMWare. 
	My approach was probably too dramatic - you probably won't have to uninstall 
	VMWare, particularly if you've got more patience than I did but I didn't feel 
	like taking the time to figure it out.
	
	I also noticed anomolies when using	Sun's VirtualBox, to get the multicast 
	traffic to be reliably transmitted between guest and host I had to bridge 
	a physical adapters so the guest OS	has its own IP addy on the network. 
	The virtual host-only network does not handle multicast traffic properly.
	
	Test code targets the built in VSTS. I made this choice based on one simple 
	fact: any developer using Visual Studio .NET 2008 will have the least 
	resistance	getting the solution to build/and run tests. At some point I may 
	put up some NUnit tests since that is my framework of choice (but don't hold 
	me to it).
	
	Some of the tests make use of the MOQ for mocking, it can be found 
	at http://code.google.com/p/moq/
	
	The IoC support built into this API requires the CommonServiceLocator found 
	at http://www.codeplex.com/CommonServiceLocator. You'll find links to your
	favorite IoC container among the links on that page.
		
	I have begun a journaler implementation that will use file-based rollover
	logs for storing raw events.
	
	Feel free to offer suggestions or assistance.
	
	~Phillip (cerebralkungfu)
	