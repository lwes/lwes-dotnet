This is the .NET API for the Light Weight Event System.

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
	like	taking the time to figure it out.
	
	The test projects in the lwes solution use the built in VSTS. I made this 
	choice based on one simple fact: any developer using Visual Studio .NET 
	2008 will have the least resistance	getting the solution to build/and run 
	tests. At some point I may put up some NUnit tests since that is my framework
	of choice (but don't hold me to it).
	
	Some of the tests make use of MOQ for mocking, it can be found 
	at http://code.google.com/p/moq/
	
	At check-in the Multicast Emitter and Listener both work in multithreaded
	mode but only the Listener works in parallel mode. I'll explain the difference
	on the website shortly.
	
	There is not a journaler implementation but one will be developed in the near
	future.
	
	Feel free to offer suggestions or assistance.
	
	~Phillip (cerebralkungfu)
	