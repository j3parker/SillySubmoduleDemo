using System.Text;
using LibGit2Sharp;

var sig = new Signature(
	name: "Jacob Parker",
	email: "jacob@solidangle.ca",
	when: DateTimeOffset.Now
);

var srcDir = Path.Combine( Environment.CurrentDirectory, "..", "..", ".." );

Repository.Init( srcDir );

var repo = new Repository( srcDir );

repo.Index.Add( ".gitignore" );
repo.Index.Add( "Program.cs" );
repo.Index.Add( "SillySubmoduleDemo.csproj" );
repo.Index.Add( "counter.txt" );
repo.Index.Add( ".gitmodules" );

repo.Commit( "Iniitial commit", sig, sig );

var prev = repo.Head.Tip;

for(int i = 1; i <= 1000; i++ ) {
	var newTreeDefn = TreeDefinition.From( repo.Head.Tip.Tree );

	var counterTxtData = new MemoryStream();
	counterTxtData.Write( Encoding.ASCII.GetBytes( i.ToString() + "\n" ) );
	counterTxtData.Position = 0;
	var counterTxtBlob = repo.ObjectDatabase.CreateBlob( counterTxtData );
	newTreeDefn.Add( "counter.txt", counterTxtBlob, Mode.NonExecutableFile );

	newTreeDefn.AddGitLink( "prev", prev.Id );

	var newTree = repo.ObjectDatabase.CreateTree( newTreeDefn );

	prev = repo.ObjectDatabase.CreateCommit( sig, sig, message: i.ToString(), newTree, parents: [prev], prettifyMessage: false );
}

repo.Reset( ResetMode.Hard, prev );
