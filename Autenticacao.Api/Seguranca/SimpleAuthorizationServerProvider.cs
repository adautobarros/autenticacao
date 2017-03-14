﻿using System;
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public IHandler<DomainNotification> Notifications;

        public SimpleAuthorizationServerProvider(IUsuarioAplicacaoServico usuarioServico)
        {
            this._usuarioServico = usuarioServico;
            this.Notifications = DomainEvent.Container.GetService<IHandler<DomainNotification>>();

        }

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            await Task.FromResult(context.Validated());
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            await Task.Run(() =>
               {

                   Usuario usuario = null;
                   try
                   {
                       usuario = _usuarioServico.Autenticar(context.UserName, context.Password);

                       if (Notifications.ExisteNotificacoes())
                       {
                           var notificacoes = Notifications.Notificar();
                           StringBuilder erros = new StringBuilder();
                           foreach (var item in notificacoes)
                           {
                               erros.Append($"{item.Value}<br/>");
                           }

                           Notifications.Dispose();

                           context.SetError("invalid_grant", erros.ToString());
                           return;
                       }
                   }
                   catch (Exception ex)
                   {

                   }

                   if (usuario == null)
                   {
                       context.SetError("invalid_grant", "Usuário ou senha inválidos");
                       return;
                   }

                   var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                   identity.AddClaim(new Claim(ClaimTypes.Name, usuario.Login.ToString()));
                   identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.Codigo.ToString()));
                   identity.AddClaim(new Claim(ClaimTypes.Email, usuario.Email));
                   identity.AddClaim(new Claim(ClaimTypesCustom.FullName, usuario.Nome));
                 

                   string[] perfis = new string[1];
                   perfis[0] = "Usuario";
                   GenericPrincipal principal = new GenericPrincipal(identity, perfis);
                   Thread.CurrentPrincipal = principal;
                   context.Validated(identity);

               });
        }
    }
}