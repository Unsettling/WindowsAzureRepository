Windows Azure Cloud Storage using the Repository pattern

Repository pattern instead of an ORM but with added Unit of Work and Specification patterns

When querying Azure Tables you will usually use the .NET client to the RESTful interface. The .NET client provides a familiar ADO.NET syntax that is easy to use and works wonderfully with LINQ. To prevent the access code becoming scattered through your code you should be collecting it into some kind of DAL. You should also be thinking about testability of your code and the simplist way to provide this is to have interfaces to your data access code. Okay, so there's nothing earth-shattering here but getting the patterns together and learning to use Azure Tables to their best is probably new to you or your project.

IRepository

What do you want to provide to every object that needs a backing store? I'd suggest searching and saving so here are the two methods every repository is going to need.

IEntityRepository

What about getting back a particular entity, making changes and saving that back? The first thing to note is that in Azure Tables an entity is stored in the properties of a Table row *but* other entities may also be stored in the same Table. So think entity and not table, which is different to how you would normally think of a repository.

Let's say for this example I want to be able to get a single entity, a range of entities, to be able to delete a given entity and even to page through a range of entities.

To keep the code cleaner I'm going to pass in the parameters as already formed predicates for my where clause. There's little advantage to using the Specification pattern here other than I think it makes the code a little more explicit.

EntityRepository

It's easy enough to pass in a context for your repository following the Unit of Work pattern. You can create this quite simply (see TableStorageContext following). You have to define which Table your entity is stored in and you want that and your context as properties of your class. I find it cleaner to manage (and easier for the next developer to implement) if that work is done in a base class, RepositoryBase.

So now we actually get to the meat of the matter and implement our TableServiceContext methods for the CRUD functionality we need. In this example I've a single Save method that uses the 'Upsert' (InsertOrMerge) functionality available in Azure since v.1.4 (2011-08). The Find method is there for convience - if it doesn't suit your query then simply don't use it.

TableStorageContext

In my ServiceDefinition config I have a CloudConnectionString. This has to be parsed to get the endpoint and account details before I can create the TableServiceContext. A couple of static methods do the job. This object also implements the Commit and Rollback methods for the Unit of Work. My Commit is implementing 'Upsert' so you may want it to be different or you may want to have different implementations of TableStorageContext that you can pass in to your Repository class depending on how it needs to talk to storage.

Further Architectural Options

I favour Uncle Bob's Clean Architecture and as such I wouldn't expose my Repository classes to other modules. I would wrap them in a further service layer that would receive and pass back Model objects. Cloud Table Storage is much more flexible than relational database storage but you have to think about it quite differently and the structure of your code will be very different to what you may be used to.
