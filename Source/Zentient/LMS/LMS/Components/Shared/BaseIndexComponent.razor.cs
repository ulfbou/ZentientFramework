using LMS.Components.Pages;
using LMS.Components.Shared;
using LMS.Core.Entities;
using LMS.Core.Identity;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Zentient.Core.Services;

public abstract class BaseComponent<TEntity, TKey>
    : ComponentBase, IBaseComponent<TEntity, Guid>
    where TEntity : class, IEntity<Guid>
    where TKey : notnull, IEquatable<TKey>
{
    [Inject] protected IServiceFactory ServiceFactory { get; set; }
    [Inject] protected NavigationManager Navigation { get; set; }
    [Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    protected bool isLoading = false;
    protected string errorMessage = string.Empty;
    protected string successMessage = string.Empty;

    public event Func<Task> OnCreate;
    public event Func<Task> OnUpdate;
    public event Func<Task> OnDelete;

    protected ICRUDService<TEntity, TKey> Service => ServiceFactory.GetService<TEntity, TKey>();

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await Service.GetAllAsync(CancellationToken.None);
    }

    public virtual async Task<TEntity> GetByIdAsync(TKey id)
    {
        return await Service.GetByIdAsync(id, CancellationToken.None);
    }

    public virtual async Task CreateAsync(TEntity entity)
    {
        await Service.CreateAsync(entity);
        if (OnCreate != null) await OnCreate.Invoke();
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
        await Service.UpdateAsync(entity);
        if (OnUpdate != null) await OnUpdate.Invoke();
    }

    public virtual async Task DeleteAsync(TKey id)
    {
        await Service.DeleteAsync(id, CancellationToken.None);
        if (OnDelete != null) await OnDelete.Invoke();
    }

    public async Task<bool> HasRoleAsync(string role)
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        return user.IsInRole(role);
    }

    public void NavigateTo(string url)
    {
        Navigation.NavigateTo(url);
    }

    public void SetLoading(bool isLoading)
    {
        this.isLoading = isLoading;
    }

    public void SetError(string message)
    {
        errorMessage = message;
        successMessage = string.Empty;
    }

    public void SetSuccess(string message)
    {
        successMessage = message;
        errorMessage = string.Empty;
    }

    protected async Task ExecuteWithLoading(Func<Task> action)
    {
        try
        {
            SetLoading(true);
            await action();
        }
        catch (Exception ex)
        {
            SetError(ex.Message);
        }
        finally
        {
            SetLoading(false);
        }
    }

    public Task<TEntity> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}

